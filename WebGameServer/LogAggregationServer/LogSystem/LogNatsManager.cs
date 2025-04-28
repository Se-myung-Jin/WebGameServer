using Dapper;
using MemoryPack;
using NATS.Client;
using NATS.Client.Internals;
using NATS.Client.JetStream;
using System.Reflection;
using DBContext = BlindServerCore.Database.DatabaseContextContainer;

namespace LogAggregationServer;

public class LogNatsManager : IDisposable
{
    private class TableFlushStatus
    {
        public ConcurrentQueue<LogBase> Queue { get; } = new();
        public DateTime LastFlushedAt { get; set; } = DateTime.UtcNow;
        public int QueueLength => Queue.Count;
    }

    private readonly IConnection _connection;
    private readonly IJetStream _jetStream;
    private readonly IJetStreamManagement _jetStreamMgmt;

    private const string _streamName = "LOG_STREAM";
    private const string _defaultSubjectPattern = "logs.*";
    private readonly ConcurrentDictionary<string, TableFlushStatus> _tableStatusMap = new();
    private readonly ConcurrentDictionary<string, (string columns, PropertyInfo[] props)> _insertCache = new();
    private CancellationTokenSource _cts;

    public LogNatsManager()
    {
        var opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = "nats://localhost:4222";

        _connection = new ConnectionFactory().CreateConnection(opts);
        _jetStream = _connection.CreateJetStreamContext();
        _jetStreamMgmt = _connection.CreateJetStreamManagementContext();

        InitOrUpdateStream(_defaultSubjectPattern);

        _cts = new CancellationTokenSource();
        StartFlushTimer(_cts.Token);
    }

    public void LogToNatsJetStream<T>(T logData) where T : LogBase
    {
        if (logData == null)
        {
            return;
        }

        var subject = $"logs.{logData.GetTableName()}";
        var data = MemoryPackSerializer.Serialize(logData);

        _jetStream.Publish(subject, data);
    }

    public void TestLogInsert()
    {
        var now = TimeUtils.GetTime();

        for (int i = 0; i < 100; i++)
        {
            var log = new LogAuthDao()
            {
                UserId = 1111,
            };

            LogToNatsJetStream(log);
        }
    }

    public void SubscribeAllLogs()
    {
        EventHandler<MsgHandlerEventArgs> handler = (sender, args) =>
        {
            try
            {
                var subject = args.Message.Subject;
                var tableName = subject.Split('.').Last();

                if (!LogTableGenerator.LogTypeMapping.TryGetValue(tableName, out Type logType))
                {
                    LogSystem.Log.Error($"Unknown log type: {tableName}");
                    return;
                }

                var logData = MemoryPackSerializer.Deserialize(logType, args.Message.Data) as LogBase;
                if (logData == null)
                {
                    LogSystem.Log.Error($"Failed to deserialize {tableName}");
                    return;
                }

                var status = _tableStatusMap.GetOrAdd(tableName, _ => new TableFlushStatus());
                status.Queue.Enqueue(logData);
            }
            catch (Exception ex)
            {
                LogSystem.Log.Error($"Error handling message: {ex}");
            }
            finally
            {
                args.Message.Ack();
            }
        };

        _jetStream.PushSubscribeAsync("logs.>", handler, false);
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    private void InitOrUpdateStream(string subject)
    {
        try
        {
            var streamInfo = _jetStreamMgmt.GetStreamInfo(_streamName);

            // 이미 존재하는 경우: Subject 없으면 추가
            if (!streamInfo.Config.Subjects.Contains(subject))
            {
                var updatedSubjects = new HashSet<string>(streamInfo.Config.Subjects) { subject };

                var updatedConfig = StreamConfiguration.Builder(streamInfo.Config)
                    .WithSubjects(updatedSubjects.ToArray())
                    .Build();

                _jetStreamMgmt.UpdateStream(updatedConfig);
                Console.WriteLine($"NATS Stream '{_streamName}' updated with new subject: {subject}");
            }
        }
        catch (NATSJetStreamException ex) when (ex.ErrorCode == 404)
        {
            // 스트림이 없을 경우 생성
            var config = StreamConfiguration.Builder()
                .WithName(_streamName)
                .WithSubjects(subject)
                .WithStorageType(StorageType.File)
                .WithMaxAge(Duration.OfHours(1))
                .Build();

            _jetStreamMgmt.AddStream(config);
            Console.WriteLine($"NATS Stream '{_streamName}' created with subject: {subject}");
        }
    }

    private void StartFlushTimer(CancellationToken token)
    {
        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await FlushAll();
                await Task.Delay(TimeSpan.FromSeconds(1), token);
            }
        }, token);
    }

    private async Task FlushAll()
    {
        var now = DateTime.UtcNow;
        var selectTables = _tableStatusMap
            .Where(kv => kv.Value.QueueLength >= 2500 || (now - kv.Value.LastFlushedAt).TotalSeconds >= 1)
            .Select(kv => kv.Key)
            .ToList();

        await Parallel.ForEachAsync(selectTables, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (table, token) =>
        {
            await FlushTable(table);
        });
    }

    private async Task FlushTable(string tableName)
    {
        if (!_tableStatusMap.TryGetValue(tableName, out var status))
        {
            return;
        }

        while (status.QueueLength >= 2500)
        {
            var batch = new List<LogBase>();
            while (batch.Count < 2500 && status.Queue.TryDequeue(out var log))
            {
                batch.Add(log);
            }

            if (batch.Count == 0)
            {
                break;
            }

            try
            {
                await InsertLogIntoMySQL(tableName, batch);
                status.LastFlushedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                LogSystem.Log.Error($"Insert failed for {tableName}, restoring logs. Error: {ex.Message}");
                foreach (var log in batch)
                {
                    status.Queue.Enqueue(log);
                }

                break;
            }
        }
    }

    private async Task InsertLogIntoMySQL(string tableName, List<LogBase> logEntries)
    {
        if (logEntries == null || logEntries.Count == 0)
        {
            return;
        }

        using (var conn = DBContext.Instance.MySql.GetConnection(MySqlKind.Write))
        {
            if (!_insertCache.TryGetValue(tableName, out var cache))
            {
                var first = logEntries[0];
                var props = first.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var column = string.Join(", ", props.Select(p => p.Name));
                cache = (column, props);
                _insertCache[tableName] = cache;
            }

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"INSERT INTO {tableName} ({cache.columns}) VALUES ");

            var valueList = new List<string>();
            var parameters = new DynamicParameters();

            for (int i = 0; i < logEntries.Count; i++)
            {
                var values = new List<string>();
                foreach (var prop in cache.props)
                {
                    string paramName = $"@{prop.Name}{i}";
                    values.Add(paramName);
                    parameters.Add(paramName, prop.GetValue(logEntries[i]));
                }
                valueList.Add($"({string.Join(", ", values)})");
            }

            sqlBuilder.Append(string.Join(", ", valueList));

            await conn.ExecuteAsync(sqlBuilder.ToString(), parameters);
        }
    }
}