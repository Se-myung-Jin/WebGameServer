using BlindServerCore.Threads;

namespace LogAggregationServer;

public class LogAggregationProcessStarter : ProcessStarter
{
    protected override async Task InitializeOthersAsync(ServiceConfig config)
    {
        new TimeJob(Global.ServiceStatusMonitor.Process, 30000, "JobThreadTest").Start();
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
