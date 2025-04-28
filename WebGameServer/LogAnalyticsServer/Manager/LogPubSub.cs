namespace LogAnalyticsServer;

public class LogPubSub : PubSubManager
{
    protected override async Task RegisterSubScribeAsync()
    {
        //await DatabaseContextContainer.Instance.Redis.RegisterSubscibeAsync(GlobalValue.GLOBAL_PUBSUB, RedisKeyBuilder.CreateKey(RedisKeyType.WORLD_STATE, ""), SubscribeRedis);
    }

    protected override void ExternalHandle(SUBSCRIPTION_COMMAND origin)
    {
        switch (origin.Protocol)
        {
            case PubSubCommandProtocol.None:
                break;
        }
    }
}
