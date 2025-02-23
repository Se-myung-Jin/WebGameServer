namespace Common;

public abstract class ServiceCommon
{
    public static ServiceConfig ServiceConfig;

    protected static async Task InitializeConfigAsync(string config)
    {
        ServiceConfig = TomlLoader.Load<ServiceConfig>($"config/{config}");

        await ServiceConfig.InitializeAsync();
    }
}
