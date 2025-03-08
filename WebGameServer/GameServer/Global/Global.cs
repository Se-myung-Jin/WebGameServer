namespace GameServer;

public class Global : ServiceCommon
{
    public static async Task InitializeAsync(string appName, string configName)
    {
        SystemGlobal.Instance.Initialize(appName, dbThreadCount: (byte)(Environment.ProcessorCount * 2));

        await InitializeConfigAsync(configName);

        ServiceStatusMonitor.Initialize(new ServerStatusDao { ServerType = EServerType.Game, PublicAddress = ServiceConfig.PublicAddress, PrivateAddress = ServiceConfig.PrivateAddress, CommitHash = GetCommitHash() });
        ServiceStatusMonitor.SetEnable(true);

        SystemGlobal.Instance.StartScheduler();
    }

    public static void Destruct()
    {
        ServiceWeb.Instance.StopAsync().Wait();
    }
}
