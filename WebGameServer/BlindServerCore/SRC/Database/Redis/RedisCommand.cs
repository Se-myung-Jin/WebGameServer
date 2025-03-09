using BlindServerCore.Log;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using DBContext = BlindServerCore.Database.DatabaseContextContainer;

namespace BlindServerCore.Database;

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
            LogSystem.Log.Error(ex);
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
            LogSystem.Log.Error(ex);
        }

        return -1;
    }
}
