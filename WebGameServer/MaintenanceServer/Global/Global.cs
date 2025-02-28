using Common;

namespace MaintenanceServer;

public class Global : ServiceCommon
{
    public static async Task InitializeAsync(string appName, string configName)
    {
        SystemGlobal.Instance.Initialize(appName, dbThreadCount: 0);

        await InitializeConfigAsync(configName);

        SystemGlobal.Instance.StartScheduler();
    }

    public static void Destruct()
    {
        ServiceWeb.Instance.StopAsync().Wait();
    }
}
