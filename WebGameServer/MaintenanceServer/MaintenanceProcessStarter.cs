namespace MaintenanceServer;

public class MaintenanceProcessStarter : ProcessStarter
{
    protected override Task OnWaitExitSignalAsync() => Task.CompletedTask;
}
