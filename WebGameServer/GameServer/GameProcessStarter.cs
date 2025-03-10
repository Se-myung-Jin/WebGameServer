using BlindServerCore.Threads;

namespace GameServer;

public class GameProcessStarter : ProcessStarter
{
    protected override async Task InitializeOthersAsync(ServiceConfig config)
    {
        new TimeJob(Global.ServiceStatusMonitor.Process, 30000, "JobThreadTest").Start();
    }

    protected override async Task InitializeServiceAsync(ServiceConfig config)
    {
        if (Global.ServiceConfig.GameConfig.Enable)
        {
            await ServiceWeb.Instance.StartAsync(Global.ServiceConfig.GameConfig.Url, null);
        }
    }

    protected override Task OnWaitExitSignalAsync() => Task.CompletedTask;
}
