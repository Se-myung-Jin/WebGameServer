using BlindServerCore.Log;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using DBContext = BlindServerCore.Database.DatabaseContextContainer;

namespace BlindServerCore.Database;

public partial class RedisCommand
{
    public static long StringIncrement(RedisStringParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            param.Value.TryParse(out long value);
            
            return db.StringIncrement(param.Key, value, flags: param.GetCommandFlags());
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }

        return -1;
    }

    public static async Task<long> StringIncrementAsync(RedisStringParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            param.Value.TryParse(out long value);

            return await db.StringIncrementAsync(param.Key, value, flags: param.GetCommandFlags());
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }

        return -1;
    }

    public static bool StringSet(RedisStringParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            return db.StringSet(param.Key, param.Value, expiry: param.ExpireSecond > 0 ? TimeSpan.FromSeconds(param.ExpireSecond) : null, flags: param.GetCommandFlags(), when: param.When);
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }

        return false;
    }

    public static async Task<bool> StringSetAsync(RedisStringParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            return await db.StringSetAsync(param.Key, param.Value, expiry: param.ExpireSecond > 0 ? TimeSpan.FromSeconds(param.ExpireSecond) : null, flags: param.GetCommandFlags());
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }

        return false;
    }

    public static RedisValue StringGet(RedisStringParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            return db.StringGet(param.Key);
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }

        return default;
    }

    public static T StringGet<T>(RedisStringParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            byte[] values = db.StringGet(param.Key);

            return values.ReadMemoryPack<T>();
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }

        return default;
    }

    public static Nullable<T> StringGeNullAblet<T>(RedisStringParameter param) where T : struct
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            byte[] values = db.StringGet(param.Key);

            return values.ReadMemoryPackNullAble<T>();
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }

        return null;
    }

    public static async Task<RedisValue> StringGetAsync(RedisStringParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            return await db.StringGetAsync(param.Key);
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }

        return default;
    }

    public static async Task<T> StringGetAsync<T>(RedisStringParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            byte[] values = await db.StringGetAsync(param.Key);

            return values.ReadMemoryPack<T>();
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }

        return default;
    }
}