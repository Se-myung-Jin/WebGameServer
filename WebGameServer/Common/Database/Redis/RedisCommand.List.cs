using StackExchange.Redis;

namespace Common;

public partial class RedisCommand
{
    public static long ListCount(RedisListParameter param)
    {
        long count = 0;
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            count = db.ListLength(param.Key);
        }
        catch (Exception ex)
        {

        }

        return count;
    }

    public static async Task<long> ListCountAsync(RedisListParameter param)
    {
        long count = 0;
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            count = await db.ListLengthAsync(param.Key);
        }
        catch (Exception ex)
        {

        }

        return count;
    }

    public static long ListAddLeft(RedisListParameter param)
    {
        long count = 0;
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            count = db.ListLeftPush(param.Key, param.Values, param.GetCommandFlags());

            if (param.ExpireSecond > 0)
            {
                _ = KeyExtendAsync(param.Kind, new RedisParameter { Kind = param.Kind, Key = param.Key, ExpireSecond = param.ExpireSecond, FireAndForget = true });
            }
        }
        catch (Exception ex)
        {

        }

        return count;
    }

    public static async Task<long> ListAddLeftAsync(RedisListParameter param)
    {
        long count = 0;
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            count = await db.ListLeftPushAsync(param.Key, param.Values, param.GetCommandFlags());

            if (param.ExpireSecond > 0)
            {
                _ = KeyExtendAsync(param.Kind, new RedisParameter { Kind = param.Kind, Key = param.Key, ExpireSecond = param.ExpireSecond, FireAndForget = true });
            }
        }
        catch (Exception ex)
        {

        }

        return count;
    }

    public static long ListRightPop(RedisListParameter param, long[] dataArray)
    {
        int count = 0;
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            var entityArray = db.ListRightPop(param.Key, param.PopCount, param.GetCommandFlags());
            count = entityArray?.Length ?? 0;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    entityArray[i].TryParse(out dataArray[i]);
                }
            }

            if (param.ExpireSecond > 0)
            {
                _ = KeyExtendAsync(param.Kind, new RedisParameter { Kind = param.Kind, Key = param.Key, ExpireSecond = param.ExpireSecond, FireAndForget = true });
            }
        }
        catch (Exception ex)
        {

        }

        return count;
    }

    public static async Task<long> ListRightPopAsync(RedisListParameter param, long[] dataArray)
    {
        int count = 0;
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            var entityArray = await db.ListRightPopAsync(param.Key, param.PopCount, param.GetCommandFlags());
            count = entityArray?.Length ?? 0;

            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    entityArray[i].TryParse(out dataArray[i]);
                }
            }

            if (param.ExpireSecond > 0)
            {
                _ = KeyExtendAsync(param.Kind, new RedisParameter { Kind = param.Kind, Key = param.Key, ExpireSecond = param.ExpireSecond, FireAndForget = true });
            }
        }
        catch (Exception ex)
        {

        }

        return count;
    }

    public static long ListRightPopFillQueue(RedisListParameter param, ConcurrentQueue<long> fillQueue)
    {
        long fillCount = 0;
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            var entityArray = db.ListRightPop(param.Key, param.PopCount);
            fillCount = entityArray?.Length ?? 0;

            if (fillCount > 0)
            {
                for (int i = 0, count = entityArray.Length; i < count; ++i)
                {
                    fillQueue.Enqueue((long)entityArray[i]);
                }
            }
        }
        catch (Exception ex)
        {
            fillCount = 0;
        }

        return fillCount;
    }

    public static async Task<long> ListRightPopFillQueueAsync(RedisListParameter param, ConcurrentQueue<long> fillQueue)
    {
        long fillCount = 0;
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            var entityArray = await db.ListRightPopAsync(param.Key, param.PopCount, param.GetCommandFlags());
            fillCount = entityArray?.Length ?? 0;

            if (fillCount > 0)
            {
                for (int i = 0, count = entityArray.Length; i < count; ++i)
                {
                    fillQueue.Enqueue((long)entityArray[i]);
                }
            }
        }
        catch (Exception ex)
        {
            fillCount = 0;
        }

        return fillCount;
    }

    public static long ListRightPopFillQueue(RedisListParameter param, ConcurrentQueue<byte[]> fillQueue)
    {
        int fillCount = 0;
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            var entityArray = db.ListRightPop(param.Key, param.PopCount, param.GetCommandFlags());
            fillCount = entityArray?.Length ?? 0;

            if (fillCount > 0)
            {
                for (int i = 0, count = entityArray.Length; i < count; ++i)
                {
                    fillQueue.Enqueue((byte[])entityArray[i]);
                }
            }
        }
        catch (Exception ex)
        {

        }

        return fillCount;
    }

    public static async Task<long> ListRightPopFillQueueAsync(RedisListParameter param, ConcurrentQueue<byte[]> fillQueue)
    {
        int fillCount = 0;
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            var entityArray = await db.ListRightPopAsync(param.Key, param.PopCount, param.GetCommandFlags());
            fillCount = entityArray?.Length ?? 0;

            if (fillCount > 0)
            {
                for (int i = 0, count = entityArray.Length; i < count; ++i)
                {
                    fillQueue.Enqueue((byte[])entityArray[i]);
                }
            }
        }
        catch (Exception ex)
        {

        }

        return fillCount;
    }

    public static List<byte[]> ListRange(RedisListParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            RedisValue[] values = db.ListRange(param.Key);

            var list = new List<byte[]>();
            foreach (var value in values)
            {
                list.Add((byte[])value);
            }
            return list;
        }
        catch (Exception ex)
        {

        }

        return null;
    }

    public static async Task<List<byte[]>> ListRangeAsync(RedisListParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            RedisValue[] values = await db.ListRangeAsync(param.Key);

            var list = new List<byte[]>();
            foreach (var value in values)
            {
                list.Add((byte[])value);
            }
            return list;
        }
        catch (Exception ex)
        {

        }

        return null;
    }

    public static void DeleteKey(RedisListParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            db.KeyDelete(param.Key);
        }
        catch (Exception ex)
        {

        }
    }

    public static async Task DeleteKeyAsync(RedisListParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            await db.KeyDeleteAsync(param.Key);
        }
        catch (Exception ex)
        {

        }
    }

    public static bool ListIsKeyExists(RedisListParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        bool isExists = false;
        try
        {
            isExists = db.KeyExists(param.Key);
        }
        catch (Exception ex)
        {

        }
        return isExists;
    }

    public static async Task<bool> ListIsKeyExistsAsync(RedisListParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        bool isExists = false;
        try
        {
            isExists = await db.KeyExistsAsync(param.Key);
        }
        catch (Exception ex)
        {

        }
        return isExists;
    }
}