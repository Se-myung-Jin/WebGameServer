using MemoryPack;

namespace Common.Database;

public static class LogWriter
{
    private static RedisParameter m_param;

    public static void ConfigureRedis(RedisParameter param)
    {
        m_param = param;
    }

    public static void LogToRedisStream<T>(T logData) where T : LogBase
    {
        if (logData == null)
        {
            return;
        }

        var data = MemoryPackSerializer.Serialize(logData);

        RedisCommand.LogToRedisStream(m_param, logData.GetTableName(), data);
    }
}
