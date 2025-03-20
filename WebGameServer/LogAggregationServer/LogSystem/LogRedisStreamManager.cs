using Dapper;
using MemoryPack;
using DBContext = BlindServerCore.Database.DatabaseContextContainer;

namespace LogAggregationServer;

public class LogRedisStreamManager
{
    RedisParameter _parameter;
    private readonly ConcurrentQueue<LogBase> _logQueue = new();


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

        foreach (var entry in entries)
        {
            string tableName = entry.Values.First(v => v.Name == "tableName").Value.ToString();
            byte[] logDataBytes = (byte[])entry.Values.First(v => v.Name == "data").Value;

            if (!LogTableGenerator.LogTypeMapping.TryGetValue(tableName, out Type logClassType))
            {
                LogSystem.Log.Error($"Unknown log type: {tableName}");
                continue;
            }

            var logData = MemoryPackSerializer.Deserialize(logClassType, logDataBytes) as LogBase;
            if (logData == null)
            {
                LogSystem.Log.Error($"Failed to cast {logClassType.Name} to LogBase");
                continue;
            }

            _logQueue.Enqueue(logData);

            await RedisCommand.ConsumeAsync(_parameter, entry.Id);
        }

        StartProcessingQueue();
    }

    private void StartProcessingQueue()
    {
        Task.Run(async () =>
        {
            while (_logQueue.Count > 0)
            {
                var batchSize = 1000;
                var batch = new List<LogBase>();

                while (batch.Count < batchSize && _logQueue.TryDequeue(out var log))
                {
                    batch.Add(log);
                }

                if (batch.Count > 0)
                {
                    string tableName = batch[0].GetTableName();
                    await InsertLogIntoMySQL(tableName, batch);
                }
            }
        });
    }

    private async Task InsertLogIntoMySQL(string tableName, List<LogBase> logEntries)
    {
        if (logEntries == null || logEntries.Count == 0)
        {
            return;
        }

        var conn = DBContext.Instance.MySql.GetConnection(MySqlKind.Write);

        string sql = GenerateBulkInsertQuery(logEntries, tableName);

        await conn.ExecuteAsync(sql, logEntries);
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
