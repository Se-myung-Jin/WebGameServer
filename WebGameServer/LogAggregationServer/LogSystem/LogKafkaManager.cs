using Confluent.Kafka;
using Dapper;
using MemoryPack;
using System.Reflection;
using DBContext = BlindServerCore.Database.DatabaseContextContainer;

namespace LogAggregationServer;

public class LogKafkaManager
{
    private class TableFlushStatus
    {
        public ConcurrentQueue<LogBase> Queue { get; } = new();
        public DateTime LastFlushedAt { get; set; } = DateTime.UtcNow;
        public int QueueLength => Queue.Count;
    }

    private readonly ConcurrentDictionary<string, TableFlushStatus> m_tableStatusMap = new();
    private readonly ConcurrentDictionary<string, (string columns, PropertyInfo[] props)> m_insertCache = new();
    private CancellationTokenSource m_cts;
    private readonly IProducer<string, byte[]> _producer;
    private readonly IConsumer<string, byte[]> _consumer;
    private readonly string _topic;

    public LogKafkaManager(string bootstrapServers = "172.30.206.137:9092", string topic = "logs", string groupId = "log-consumer-group")
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            BatchSize = 32768,
            LingerMs = 10,
            CompressionType = CompressionType.Snappy
        };

        _producer = new ProducerBuilder<string, byte[]>(config).Build();

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            MaxPollIntervalMs = 300000,
            FetchMaxBytes = 52428800,
            FetchMinBytes = 1,
            FetchWaitMaxMs = 100,
        };

        _consumer = new ConsumerBuilder<string, byte[]>(consumerConfig).Build();

        _topic = topic;

        m_cts = new CancellationTokenSource();
        StartFlushTimer(m_cts.Token);
        ConsumeKafkaLogsAsync();
    }

    public async Task LogToKafka<T>(T logData) where T : LogBase
    {
        if (logData == null)
            return;

        var topic = "logs";
        var key = $"{logData.GetTableName()}";
        var data = MemoryPackSerializer.Serialize(logData);

        try
        {
            var message = new Message<string, byte[]>
            {
                Key = key,
                Value = data,
            };

            var result = await _producer.ProduceAsync(topic, message);
        }
        catch (ProduceException<Null, byte[]> ex)
        {
            Console.WriteLine($"[Kafka] Produce failed: {ex.Error.Reason}");
        }
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

            _ = LogToKafka(log);
        }
    }

    // Kafka는 batch consume 이 없다.
    public void ConsumeKafkaLogsAsync()
    {
        Task.Run(() =>
        {
            _consumer.Subscribe(_topic);
            Console.WriteLine($"[Kafka] Started consuming topic: {_topic}");

            try
            {
                while (!m_cts.Token.IsCancellationRequested)
                {
                    var batch = new List<ConsumeResult<string, byte[]>>();
                    var endTime = DateTime.UtcNow.AddMilliseconds(100);

                    while (DateTime.UtcNow < endTime && batch.Count < 1000)
                    {
                        var result = _consumer.Consume(TimeSpan.FromMilliseconds(10));
                        if (result != null)
                            batch.Add(result);
                    }

                    Parallel.ForEach(batch, result =>
                    {
                        string tableName = result.Message.Key;
                        byte[] logDataBytes = result.Message.Value;

                        if (!LogTableGenerator.LogTypeMapping.TryGetValue(tableName, out Type logClassType))
                            return;

                        var logData = MemoryPackSerializer.Deserialize(logClassType, logDataBytes) as LogBase;
                        if (logData == null)
                            return;

                        var status = m_tableStatusMap.GetOrAdd(tableName, _ => new TableFlushStatus());
                        status.Queue.Enqueue(logData);
                    });

                    if (batch.Count > 0)
                    {
                        _consumer.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Log.Error($"[Kafka] Consumer error: {ex.Message}");
            }
            finally
            {
                _consumer.Close();
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
