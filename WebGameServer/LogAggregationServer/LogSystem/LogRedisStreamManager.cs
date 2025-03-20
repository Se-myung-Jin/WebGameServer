using Dapper;
using MemoryPack;
using DBContext = BlindServerCore.Database.DatabaseContextContainer;

namespace LogAggregationServer;

public class LogRedisStreamManager
{
    RedisParameter _parameter;
    private readonly ConcurrentDictionary<string, List<LogBase>> _logBatches = new();
    private readonly object _lock = new();

    public LogRedisStreamManager(RedisParameter parameter)
    {
        this._parameter = parameter;
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
        if (entries.Length == 0) return;

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

                lock (_lock)
                {
                    if (!_logBatches.ContainsKey(tableName))
                    {
                        _logBatches[tableName] = new List<LogBase>();
                    }
                    _logBatches[tableName].Add(logData);
                }

                if (_logBatches[tableName].Count >= 2500)
                {
                    List<LogBase> batchToInsert;
                    lock (_lock)
                    {
                        batchToInsert = new List<LogBase>(_logBatches[tableName]);
                        _logBatches[tableName].Clear();
                    }

                    await InsertLogIntoMySQL(tableName, batchToInsert);
                }

                await RedisCommand.ConsumeAsync(_parameter, entry.Id);
            }
            catch (Exception ex)
            {
                LogSystem.Log.Error($"Error processing log: {ex}");
            }
        });

        foreach (var logDatasEachTable in _logBatches)
        {
            if (logDatasEachTable.Value.Count > 0)
            {
                await InsertLogIntoMySQL(logDatasEachTable.Key, logDatasEachTable.Value);
            }
        }
    }

    private async Task InsertLogIntoMySQL(string tableName, List<LogBase> logEntries)
    {
        if (logEntries == null || logEntries.Count == 0)
        {
            return;
        }

        var conn = DBContext.Instance.MySql.GetConnection(MySqlKind.Write);

        var logEntriesCopy = new List<LogBase>(logEntries);
        string sql = GenerateBulkInsertQuery(logEntriesCopy, tableName);

        await conn.ExecuteAsync(sql, logEntriesCopy);
    }

    private string GenerateBulkInsertQuery(List<LogBase> logEntries, string tableName)
    {
        var firstEntry = logEntries.First();
        var properties = firstEntry.GetType().GetProperties();

        string columns = string.Join(", ", properties.Select(p => p.Name));
        string values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

        return $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
    }
}
