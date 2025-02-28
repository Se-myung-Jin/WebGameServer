namespace Common;

public enum ERedisKeyType
{
    Auth,
};

public static class RedisKeyBuilder
{
    static ThreadLocal<StringBuilder> _ThreadGlobalKeyBuilder = new ThreadLocal<StringBuilder>(() => new StringBuilder());
    static StringBuilder _globalKeyBuilder => _ThreadGlobalKeyBuilder.Value;

    static Dictionary<ERedisKeyType, string> _globalKeyMap = new()
    {
        {ERedisKeyType.Auth, "AUTH_" },
    };

    public static string GetKey(ERedisKeyType keyType) => _globalKeyMap[keyType];

    public static string CreateKey(ERedisKeyType keyType, string combineValue)
    {
        _globalKeyBuilder.Clear();
        _globalKeyBuilder.Append(_globalKeyMap[keyType]);
        _globalKeyBuilder.Append(combineValue);
        return _globalKeyBuilder.ToString();
    }

    public static string CreateKey(ERedisKeyType keyType, long combineValue)
    {
        _globalKeyBuilder.Clear();
        _globalKeyBuilder.Append(_globalKeyMap[keyType]);
        _globalKeyBuilder.Append(combineValue);
        return _globalKeyBuilder.ToString();
    }

    public static string CreateKey(ERedisKeyType keyType, int combineValue)
    {
        _globalKeyBuilder.Clear();
        _globalKeyBuilder.Append(_globalKeyMap[keyType]);
        _globalKeyBuilder.Append(combineValue);
        return _globalKeyBuilder.ToString();
    }
}