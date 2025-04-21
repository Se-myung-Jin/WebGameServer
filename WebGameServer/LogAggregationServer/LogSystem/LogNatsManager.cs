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

    private readonly IConnection m_connection;
    private readonly IJetStream m_jetStream;
    private readonly IJetStreamManagement m_jetStreamMgmt;

    private const string m_streamName = "LOG_STREAM";
    private const string m_defaultSubjectPattern = "logs.*";
    private readonly ConcurrentDictionary<string, TableFlushStatus> m_tableStatusMap = new();
    private readonly ConcurrentDictionary<string, (string columns, PropertyInfo[] props)> m_insertCache = new();
    private CancellationTokenSource m_cts;

    public LogNatsManager()
    {
        var opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = "nats://localhost:4222";

        m_connection = new ConnectionFactory().CreateConnection(opts);
        m_jetStream = m_connection.CreateJetStreamContext();
        m_jetStreamMgmt = m_connection.CreateJetStreamManagementContext();

        InitOrUpdateStream(m_defaultSubjectPattern);

        m_cts = new CancellationTokenSource();
        StartFlushTimer(m_cts.Token);
    }

    public void LogToNatsJetStream<T>(T logData) where T : LogBase
    {
        if (logData == null)
        {
            return;
        }

        var subject = $"logs.{logData.GetTableName()}";
        var data = MemoryPackSerializer.Serialize(logData);

        m_jetStream.Publish(subject, data);
    }

    public void TestLogInsert()
    {
        var now = TimeUtils.GetTime();

        for (int i = 0; i < 100; i++)
        {
            var log = new LogItemGetDao()
            {
                UserId = 1111,
                ItemId = 2222,
                Quantity = 3,
                ObtainedAt = now
            };

            LogToNatsJetStream(log);
        }

        for (int i = 0; i < 100; i++)
        {
            var log = new LogItemRemoveDao()
            {
                Id = i,
                LogLevel = "warn",
                Message = "Hello",
                RemovedAt = now
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

                var status = m_tableStatusMap.GetOrAdd(tableName, _ => new TableFlushStatus());
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

        m_jetStream.PushSubscribeAsync("logs.>", handler, false);
    }

    public void Dispose()
    {
        m_connection?.Dispose();
    }

    private void InitOrUpdateStream(string subject)
    {
        try
        {
            var streamInfo = m_jetStreamMgmt.GetStreamInfo(m_streamName);

            // 이미 존재하는 경우: Subject 없으면 추가
            if (!streamInfo.Config.Subjects.Contains(subject))
            {
                var updatedSubjects = new HashSet<string>(streamInfo.Config.Subjects) { subject };

                var updatedConfig = StreamConfiguration.Builder(streamInfo.Config)
                    .WithSubjects(updatedSubjects.ToArray())
                    .Build();

                m_jetStreamMgmt.UpdateStream(updatedConfig);
                Console.WriteLine($"NATS Stream '{m_streamName}' updated with new subject: {subject}");
            }
        }
        catch (NATSJetStreamException ex) when (ex.ErrorCode == 404)
        {
            // 스트림이 없을 경우 생성
            var config = StreamConfiguration.Builder()
                .WithName(m_streamName)
                .WithSubjects(subject)
                .WithStorageType(StorageType.File)
                .WithMaxAge(Duration.OfHours(1))
                .Build();

            m_jetStreamMgmt.AddStream(config);
            Console.WriteLine($"NATS Stream '{m_streamName}' created with subject: {subject}");
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
        var selectTables = m_tableStatusMap
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
        if (!m_tableStatusMap.TryGetValue(tableName, out var status))
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
            if (!m_insertCache.TryGetValue(tableName, out var cache))
            {
                var first = logEntries[0];
                var props = first.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var column = string.Join(", ", props.Select(p => p.Name));
                cache = (column, props);
                m_insertCache[tableName] = cache;
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