﻿namespace Common;

public partial class RedisCommand
{
    public static bool TakeLock(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            return db.LockTake(param.Key, param.Value, TimeSpan.FromSeconds(param.ExpireSecond));
        }
        catch (Exception ex)
        {

        }

        return false;
    }

    public static async Task<bool> TakeLockAsync(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            return await db.LockTakeAsync(param.Key, param.Value, TimeSpan.FromSeconds(param.ExpireSecond));
        }
        catch (Exception ex)
        {

        }

        return false;
    }

    public static bool ReleaseLock(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            return db.LockRelease(param.Key, param.Value);
        }
        catch (Exception ex)
        {

        }

        return false;
    }

    public static async Task<bool> ReleaseLockAsync(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            return await db.LockReleaseAsync(param.Key, param.Value);
        }
        catch (Exception ex)
        {

        }

        return false;
    }
}
