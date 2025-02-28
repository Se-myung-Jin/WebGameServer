namespace MaintenanceServer;

public class MaintenanceProcessStarter : ProcessStarter
{
    protected override async Task InitializeOthersAsync(ServiceConfig config)
    {
        new TimeJob(Global.ServiceStatusMonitor.Process, 3000, "JobThreadTest").Start();
    }

    protected override async Task InitializeServiceAsync(ServiceConfig config)
    {
        if (Global.ServiceConfig.MaintenanceConfig.Enable)
        {
            await ServiceWeb.Instance.StartAsync(Global.ServiceConfig.MaintenanceConfig.Url, null);
        }
    }

    protected override Task OnWaitExitSignalAsync() => Task.CompletedTask;
}
