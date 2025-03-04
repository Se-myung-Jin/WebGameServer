using StackExchange.Redis;

namespace Common;

public partial class RedisCommand
{
    public static async Task<bool> HashSetAsync(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            await db.HashSetAsync(param.Key, param.HashEntityArray);
            if (param.ExpireSecond > 0)
            {
                await KeyExtendAsync(db, param);
            }

            return true;
        }
        catch (Exception ex)
        {

        }

        return false;
    }

    public static async Task<bool> HashSetAsync(RedisParameter param, bool keyExist = false)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            if (keyExist)
            {
                if (await KeyExtendAsync(db, param) == false)
                {
                    return false;
                }
            }

            await db.HashSetAsync(param.Key, param.Member, param.Value);
            await KeyExtendAsync(db, param);

            return true;
        }
        catch (Exception ex)
        {
            
        }

        return false;
    }

    public static async Task<bool> HashRemoveAsync(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            await db.HashDeleteAsync(param.Key, param.Member);
            if (param.ExpireSecond > 0)
            {
                await KeyExtendAsync(db, param);
            }

            return true;
        }
        catch (Exception ex)
        {
            
        }

        return false;
    }

    public static async Task<HashEntry[]> HashGetAllAsync(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            return await db.HashGetAllAsync(param.Key);
        }
        catch (Exception ex)
        {
            
        }

        return null;
    }
}