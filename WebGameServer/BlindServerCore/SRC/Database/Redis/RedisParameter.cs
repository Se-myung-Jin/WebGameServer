using StackExchange.Redis;

namespace BlindServerCore.Database;

public readonly struct RedisParameter
{
    public readonly int Kind { get; init; }
    public readonly RedisKey Key { get; init; }
    public readonly RedisValue Value { get; init; }
    public readonly RedisValue Member { get; init; }
    public readonly HashEntry[] HashEntityArray { get; init; }
    public readonly HashEntry HashEntity { get; init; }

    public readonly uint ExpireSecond { get; init; }
    public readonly uint KeyExtendSecondCondition { get; init; }
    public readonly int DataCount { get; init; }
    public readonly bool FireAndForget { get; init; }
    public readonly bool ReadAndDelete { get; init; }

    public CommandFlags GetCommandFlags()
    {
        return FireAndForget ? CommandFlags.FireAndForget : CommandFlags.None;
    }
}

public readonly struct RedisStringParameter
{
    public readonly int Kind { get; init; }
    public readonly RedisKey Key { get; init; }
    public readonly RedisValue Value { get; init; }
    public readonly When When { get; init; }
    public readonly uint ExpireSecond { get; init; }
    public readonly uint KeyExtendSecondCondition { get; init; }
    public readonly bool FireAndForget { get; init; }
    public readonly bool ReadAndDelete { get; init; }

    public CommandFlags GetCommandFlags()
    {
        return FireAndForget ? CommandFlags.FireAndForget : CommandFlags.None;
    }
};

public readonly struct RedisListParameter
{
    public readonly int Kind { get; init; }
    public readonly RedisKey Key { get; init; }
    public readonly RedisValue[] Values { get; init; }
    public readonly bool FireAndForget { get; init; }
    public readonly ushort PopCount { get; init; }
    public readonly uint ExpireSecond { get; init; }

    public CommandFlags GetCommandFlags()
    {
        return FireAndForget ? CommandFlags.FireAndForget : CommandFlags.None;
    }
};
