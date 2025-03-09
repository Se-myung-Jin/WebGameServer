using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BlindServerCore.Database;

public static class RedisKeyBuilder
{
    static ThreadLocal<StringBuilder> _ThreadGlobalKeyBuilder = new ThreadLocal<StringBuilder>(() => new StringBuilder());
    static StringBuilder _globalKeyBuilder => _ThreadGlobalKeyBuilder.Value;

    static Dictionary<RedisKeyType, string> _globalKeyMap = new()
    {
        {RedisKeyType.Auth, "AUTH_" },
    };

    public static string GetKey(RedisKeyType keyType) => _globalKeyMap[keyType];

    public static string CreateKey(RedisKeyType keyType, string combineValue)
    {
        _globalKeyBuilder.Clear();
        _globalKeyBuilder.Append(_globalKeyMap[keyType]);
        _globalKeyBuilder.Append(combineValue);
        return _globalKeyBuilder.ToString();
    }

    public static string CreateKey(RedisKeyType keyType, long combineValue)
    {
        _globalKeyBuilder.Clear();
        _globalKeyBuilder.Append(_globalKeyMap[keyType]);
        _globalKeyBuilder.Append(combineValue);
        return _globalKeyBuilder.ToString();
    }

    public static string CreateKey(RedisKeyType keyType, int combineValue)
    {
        _globalKeyBuilder.Clear();
        _globalKeyBuilder.Append(_globalKeyMap[keyType]);
        _globalKeyBuilder.Append(combineValue);
        return _globalKeyBuilder.ToString();
    }
}