namespace MaintenanceServer;

public class MaintenanceProcessStarter : ProcessStarter
{
    protected override async Task InitializeServiceAsync(ServiceConfig config)
    {
        await ServiceWeb.Instance.StartAsync(Global.ServiceConfig.MaintenanceConfig.Url, null);
    }
    protected override Task OnWaitExitSignalAsync() => Task.CompletedTask;
}
