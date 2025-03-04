using StackExchange.Redis;
using System.Runtime.CompilerServices;

namespace Common;

public class RedisContextContainer
{
    private Dictionary<int, ConnectionMultiplexer> _connectionMap = new();

    public void Initialize()
    {
    }

    public ConnectionMultiplexer Add(DBConfig config)
    {
        var client = ConnectionMultiplexer.Connect(config.ConnectionString);
        _connectionMap.Add(config.Id, client);

        var elapse = PingTest(client);
        if (elapse < 0)
        {
            throw new Exception($"Ping Fail Exception. Address: {config.ConnectionString}");
        }

        return client;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConnectionMultiplexer Client(int id) => _connectionMap[id];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDatabase GetDb(int id, int db = -1) => Client(id).GetDatabase(db);

    public ISubscriber GetSubscriber(int id) => Client(id).GetSubscriber();

    public void RegisterSubscibe(int id, string channel, Action<RedisChannel, RedisValue> handler, bool fireAndForget = false)
    {
        GetSubscriber(id).Subscribe(RedisChannel.Literal(channel), handler, flags: CreateFlag(fireAndForget));
    }

    public async Task RegisterSubscibeAsync(int id, string channel, Action<RedisChannel, RedisValue> handler, bool fireAndForget = false)
    {
        await GetSubscriber(id).SubscribeAsync(RedisChannel.Literal(channel), handler, flags: CreateFlag(fireAndForget));
    }

    public void UnRegisterSubscibe(int id, string channel, Action<RedisChannel, RedisValue> handler = null, bool fireAndForget = false)
    {
        GetSubscriber(id).Unsubscribe(RedisChannel.Literal(channel), flags: CreateFlag(fireAndForget));
    }

    public async Task UnRegisterSubscibeAsync(int id, string channel, Action<RedisChannel, RedisValue> handler = null, bool fireAndForget = false)
    {
        await GetSubscriber(id).UnsubscribeAsync(RedisChannel.Literal(channel), handler, flags: CreateFlag(fireAndForget));
    }

    public void UnRegisterSubscibeAll(int id, string channel, Action<RedisChannel, RedisValue> handler = null, bool fireAndForget = true)
    {
        GetSubscriber(id).UnsubscribeAll(CreateFlag(fireAndForget));
    }

    public async Task UnRegisterSubscibeAllAsync(int id, RedisChannel channel, bool fireAndForget = true)
    {
        await GetSubscriber(id).UnsubscribeAllAsync(CreateFlag(fireAndForget));
    }

    public void Publish(int id, string channel, RedisValue message, bool fireAndForget = false)
    {
        GetSubscriber(id).Publish(RedisChannel.Literal(channel), message, flags: CreateFlag(fireAndForget));
    }

    public async Task PublishAsync(int id, string channel, RedisValue message, bool fireAndForget = false)
    {
        await GetSubscriber(id).PublishAsync(RedisChannel.Literal(channel), message, flags: CreateFlag(fireAndForget));
    }

    public void Destory()
    {
        foreach (var connection in _connectionMap.Values)
        {
            connection.Dispose();
        }

        _connectionMap.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private CommandFlags CreateFlag(bool fireAndForget) => fireAndForget ? CommandFlags.FireAndForget : CommandFlags.None;

    private int PingTest(IConnectionMultiplexer connection)
    {
        try
        {
            var timeSpan = connection.GetDatabase().Ping();

            return timeSpan.Milliseconds;
        }
        catch (Exception ex)
        {

        }

        return -1;
    }
}