using BlindServerCore.Threads;

namespace ScheduleServer;

public class ScheduleProcessStarter : ProcessStarter
{
    protected override async Task InitializeOthersAsync(ServiceConfig config)
    {
        new TimeJob(Global.ServiceStatusMonitor.Process, 30000, "ServiceStatusMonitor").Start();
    }

    protected override async Task InitializeServiceAsync(ServiceConfig config)
    {
        if (Global.ServiceConfig.ScheduleConfig.Enable)
        {
            await ServiceWeb.Instance.StartAsync(Global.ServiceConfig.ScheduleConfig.Url, null);
        }
    }

    protected override Task OnWaitExitSignalAsync() => Task.CompletedTask;
}
