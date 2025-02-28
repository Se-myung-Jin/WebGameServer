using System.Reflection;

namespace Common;

public abstract class ServiceCommon
{
    public static ServiceStatusMonitor ServiceStatusMonitor = new();
    public static ServiceConfig ServiceConfig;

    public static string GetCommitHash()
    {
        string versionFullName = Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        string versionSuffix = "";

        int startIndex = versionFullName.IndexOf('-');
        //NOTE: GIT API 추가 시 + 이후에 LONG_HASH값이 추가되는 경우 있음
        //int endIndex = versionFullName.IndexOf('+');
        if (startIndex > 0)
        {
            versionSuffix = versionFullName.Substring(startIndex + 1);
        }

        return versionSuffix;
    }

    protected static async Task InitializeConfigAsync(string config)
    {
        ServiceConfig = TomlLoader.Load<ServiceConfig>($"config/{config}");

        await ServiceConfig.InitializeAsync();
    }
}
