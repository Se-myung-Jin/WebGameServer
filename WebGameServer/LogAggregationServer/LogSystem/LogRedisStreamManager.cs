using Dapper;
using MemoryPack;
using System.Reflection;
using DBContext = BlindServerCore.Database.DatabaseContextContainer;

namespace LogAggregationServer;

public class LogRedisStreamManager
{
    private class TableFlushStatus
    {
        public ConcurrentQueue<LogBase> Queue { get; } = new();
        public DateTime LastFlushedAt { get; set; } = DateTime.UtcNow;
        public int QueueLength => Queue.Count;
    }

    RedisParameter _parameter;
    private readonly ConcurrentDictionary<string, TableFlushStatus> _tableStatusMap = new();
    private readonly ConcurrentDictionary<string, (string columns, PropertyInfo[] props)> _insertCache = new();
    private CancellationTokenSource _cts;

    public LogRedisStreamManager(RedisParameter parameter)
    {
        this._parameter = parameter;
        _cts = new CancellationTokenSource();
        StartFlushTimer(_cts.Token);
    }

    public void Dispose()
    {
        _cts?.Cancel();
    }

    public void TestLogInsert()
    {
        var now = TimeUtils.GetTime();
        for (int i = 0; i < 10000; i++)
        {
            var log = new LogItemGetDao()
            {
                UserId = 1111,
                ItemId = 2222,
                Quantity = 3,
                ObtainedAt = now
            };

            LogToRedisStream(log);
        }
    }

    public void CreateLogRedisStreamGroup()
    {
        RedisCommand.StreamCreateGroup(_parameter);
    }

    public void LogToRedisStream<T>(T logData) where T : LogBase
    {
        var data = MemoryPackSerializer.Serialize(logData);
        
        RedisCommand.LogToRedisStream(_parameter, logData.GetTableName(), data);
    }

    public async Task ConsumeLogsAsync()
    {
        var entries = await RedisCommand.StreamGroupReadAsync(_parameter);
        if (entries == null) return;

        await Parallel.ForEachAsync(entries, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (entry, token) =>
        {
            try
            {
                string tableName = entry.Values.First(v => v.Name == "tableName").Value.ToString();
                byte[] logDataBytes = (byte[])entry.Values.First(v => v.Name == "data").Value;

                if (!LogTableGenerator.LogTypeMapping.TryGetValue(tableName, out Type logClassType))
                {
                    LogSystem.Log.Error($"Unknown log type: {tableName}");
                    return;
                }

                var logData = MemoryPackSerializer.Deserialize(logClassType, logDataBytes) as LogBase;
                if (logData == null)
                {
                    LogSystem.Log.Error($"Failed to cast {logClassType.Name} to LogBase");
                    return;
                }

                var status = _tableStatusMap.GetOrAdd(tableName, _ => new TableFlushStatus());
                status.Queue.Enqueue(logData);

                await RedisCommand.ConsumeAsync(_parameter, entry.Id);
            }
            catch (Exception ex)
            {
                LogSystem.Log.Error($"Error processing log: {ex}");
            }
        });
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
