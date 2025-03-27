using BlindServerCore.Threads;

namespace LogAggregationServer;

public class LogAggregationProcessStarter : ProcessStarter
{
    protected override async Task InitializeOthersAsync(ServiceConfig config)
    {
        RedisParameter param = new RedisParameter() { Key = "log_stream", Value = "log_group", Member = "consumer1", DataCount = 10000, Kind = GlobalValue.GLOBAL_REDIS };
        LogRedisStreamManager redis = new LogRedisStreamManager(param);
        redis.CreateLogRedisStreamGroup();

        new TimeJob(Global.ServiceStatusMonitor.Process, 30000, "JobThreadTest").Start();
        new TimeJob(async () => await redis.ConsumeLogsAsync(), 1000, "consume1").Start();
    }

    protected override async Task InitializeServiceAsync(ServiceConfig config)
    {
        if (Global.ServiceConfig.LogAggregationConfig.Enable)
        {
            await ServiceWeb.Instance.StartAsync(Global.ServiceConfig.LogAggregationConfig.Url, null);
        }
    }

    protected override Task OnWaitExitSignalAsync() => Task.CompletedTask;
}
