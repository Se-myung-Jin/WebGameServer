using StackExchange.Redis;

namespace Common;

public partial class RedisCommand
{
    public static long Publish(int id, RedisChannel channle, RedisValue value)
    {
        var db = DBContext.Instance.Redis.GetSubscriber(id);
        try
        {
            return db.Publish(channle, value);
        }
        catch (Exception ex)
        {

        }

        return -1;
    }

    public static async Task<long> PublishAsync(int id, RedisChannel channle, RedisValue value)
    {
        var db = DBContext.Instance.Redis.GetSubscriber(id);
        try
        {
            return await db.PublishAsync(channle, value);
        }
        catch (Exception ex)
        {

        }

        return -1;
    }
}
