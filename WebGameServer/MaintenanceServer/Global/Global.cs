namespace MaintenanceServer;

public class Global : ServiceCommon
{
    public static async Task InitializeAsync(string appName, string configName)
    {
        SystemGlobal.Instance.Initialize(appName, dbThreadCount: 0);

        await InitializeConfigAsync(configName);

        ServiceStatusMonitor.Initialize(new ServerStatusDao { ServerType = EServerType.Maintenance, PublicAddress = ServiceConfig.PublicAddress, PrivateAddress = ServiceConfig.PrivateAddress, CommitHash = GetCommitHash() });

        SystemGlobal.Instance.StartScheduler();
    }

    public static void Destruct()
    {
        ServiceWeb.Instance.StopAsync().Wait();
    }
}
