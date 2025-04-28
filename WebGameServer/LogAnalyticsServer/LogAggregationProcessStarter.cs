using BlindServerCore.Threads;

namespace LogAnalyticsServer;

public class LogAnalyticsProcessStarter : ProcessStarter
{
    protected override async Task InitializeOthersAsync(ServiceConfig config)
    {
        new TimeJob(Global.ServiceStatusMonitor.Process, 30000, "ServiceStatusMonitor").Start();

        await Global.PostProcessAsync();
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
