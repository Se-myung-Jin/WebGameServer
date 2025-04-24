using BlindServerCore.Threads;

namespace MaintenanceServer;

public class MaintenanceProcessStarter : ProcessStarter
{
    protected override async Task InitializeOthersAsync(ServiceConfig config)
    {
        PlayerDataDeleteManager.Instance.Initialize();
        LogWriter.ConfigureRedis(new RedisParameter() { Key = "log_stream", Value = "log_group", Kind = GlobalValue.CACHE_REDIS });

        new TimeJob(PlayerDataDeleteManager.Instance.Process, 10000, "PlayerDataDeleteManager.Instance.Process").Start();
        new TimeJob(Global.ServiceStatusMonitor.Process, 30000, "JobThreadTest").Start();
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
