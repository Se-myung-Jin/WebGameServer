using BlindServerCore.Log;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using DBContext = BlindServerCore.Database.DatabaseContextContainer;

namespace BlindServerCore.Database;

public partial class RedisCommand
{
    public static bool KeyExtend(int id, RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(id);
        
        return KeyExtend(db, param);
    }

    public static async Task<bool> KeyExtendAsync(int id, RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(id);
        
        return await KeyExtendAsync(db, param);
    }

    public static async Task<bool> KeyDeleteAsync(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            return await db.KeyDeleteAsync(param.Key);
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }

        return false;
    }

    public static bool KeyExtend(IDatabase db, RedisParameter param)
    {
        if (param.ExpireSecond <= 0)
        {
            return false;
        }

        try
        {
            if (param.KeyExtendSecondCondition > 0)
            {
                var timeToLive = db.KeyTimeToLive(param.Key);
                if (timeToLive == null)
                {
                    return false;
                }

                if (timeToLive.Value.TotalSeconds > param.KeyExtendSecondCondition)
                {
                    return true;
                }
            }

            return db.KeyExpire(param.Key, TimeSpan.FromSeconds(param.ExpireSecond));
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }
        return false;
    }

    public static async Task<bool> KeyExtendAsync(IDatabase db, RedisParameter param)
    {
        if (param.ExpireSecond <= 0)
        {
            return false;
        }

        try
        {
            if (param.KeyExtendSecondCondition > 0)
            {
                var timeToLive = await db.KeyTimeToLiveAsync(param.Key);
                if (timeToLive == null)
                {
                    return false;
                }

                if (timeToLive.Value.TotalSeconds > param.KeyExtendSecondCondition)
                {
                    return true;
                }
            }

            return await db.KeyExpireAsync(param.Key, TimeSpan.FromSeconds(param.ExpireSecond));
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }
        return false;
    }
}