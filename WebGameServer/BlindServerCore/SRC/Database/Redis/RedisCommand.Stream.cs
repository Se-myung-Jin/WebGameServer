using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using DBContext = BlindServerCore.Database.DatabaseContextContainer;

namespace BlindServerCore.Database;

public partial class RedisCommand
{
    public static bool StreamCreateGroup(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            if (StreamExistsGroup(param))
            {
                return true;
            }

            return db.StreamCreateConsumerGroup(param.Key, param.Member);
        }
        catch (Exception ex)
        {

        }
        return false;
    }

    public static bool StreamExistsGroup(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            var groupArray = db.StreamGroupInfo(param.Key);
            if (groupArray?.Length > 0)
            {
                for (int i = 0; i < groupArray.Length; ++i)
                {
                    var group = groupArray[i];
                    if (group.Name.Equals(param.Member))
                    {
                        return true;
                    }
                }
            }
        }
        catch (RedisException redisEx)
        {
            if (string.Equals(redisEx.Message, "ERR no such key") == false)
            {

            }
        }
        catch (Exception ex)
        {

        }

        return false;
    }

    public static async Task<bool> StreamCreateGroupAsync(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            return await db.StreamCreateConsumerGroupAsync(param.Key, param.Member);
        }
        catch (Exception ex)
        {

        }
        return false;
    }

    public static string StreamAdd(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            if (param.HashEntityArray == null)
            {
                return db.StreamAdd(param.Key, param.Member, param.Value, flags: param.GetCommandFlags());
            }
            else
            {
                NameValueEntry[] dataArray = new NameValueEntry[param.HashEntityArray.Length];

                for (int i = 0; i < param.HashEntityArray.Length; ++i)
                {
                    var entity = param.HashEntityArray[i];
                    dataArray[i] = new NameValueEntry(entity.Name, entity.Value);
                }
                
                return db.StreamAdd(param.Key, dataArray, flags: param.GetCommandFlags());
            }
        }
        catch (Exception ex)
        {

        }

        return default;
    }

    public static async Task<string> StreamAddAsync(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            if (param.HashEntityArray == null)
            {
                return await db.StreamAddAsync(param.Key, param.Member, param.Value, flags: param.GetCommandFlags());
            }
            else
            {
                NameValueEntry[] dataArray = new NameValueEntry[param.HashEntityArray.Length];

                for (int i = 0; i < param.HashEntityArray.Length; ++i)
                {
                    var entity = param.HashEntityArray[i];
                    dataArray[i] = new NameValueEntry(entity.Name, entity.Value);
                }

                return await db.StreamAddAsync(param.Key, dataArray, flags: param.GetCommandFlags());
            }
        }
        catch (Exception ex)
        {

        }

        return default;
    }


    public static StreamEntry[] StreamGroupRead(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            //todo:noAck 옵션을 true로 넘겨야 pending에 쌓이지 않는다.
            StreamEntry[] entityArray = db.StreamReadGroup(param.Key, param.Value, param.Member, count: param.DataCount, noAck: true);

            if (entityArray?.Length > 0)
            {
                var messasgeIdList = new RedisValue[entityArray.Length];
                for (int i = 0; i < entityArray.Length; ++i)
                {
                    messasgeIdList[i] = entityArray[i].Id;
                }

                if (param.ReadAndDelete)
                {
                    var deleteValue = db.StreamDelete(param.Key, messasgeIdList);
                }

                return entityArray;
            }
        }
        catch (Exception ex)
        {

        }

        return default;
    }

    public static async Task<StreamEntry[]> StreamGroupReadAsync(RedisParameter param)
    {
        var db = DBContext.Instance.Redis.GetDb(param.Kind);
        try
        {
            var entityArray = await db.StreamReadGroupAsync(param.Key, param.Value, param.Member, count: param.DataCount, noAck: true);
            if (entityArray?.Length > 0)
            {
                var messasgeIdList = new RedisValue[entityArray.Length];
                for (int i = 0; i < entityArray.Length; ++i)
                {
                    messasgeIdList[i] = entityArray[i].Id;
                }

                if (param.ReadAndDelete)
                {
                    await db.StreamDeleteAsync(param.Key, messasgeIdList);
                }

                return entityArray;
            }
        }
        catch (Exception ex)
        {

        }

        return default;
    }
}