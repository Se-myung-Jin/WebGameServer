namespace Common;

public class MaintenanceConfig
{
    public string Url { get; set; }
    public string Domain { get; set; }
    public bool Enable { get; set; }
};

public class ConfigDatabase
{
    public string Name { get; set; }
    public string Server { get; set; }
    public string Category { get; set; }
    public string ReplicaType { get; set; }

    public DBConfig DbPoolConfig()
    {
        Enum.TryParse<EDataBaseCategory>(Category, out var selectCategory);
        Enum.TryParse<EReplicaType>(ReplicaType, out var selectReplica);

        return new DBConfig(Name, selectCategory, selectReplica, Server, 0);
    }
}

public class ServiceConfig
{
    public bool UsePublicAddress { get; set; }
    public string PublicAddress { get; private set; }
    public string PrivateAddress { get; private set; }
    public MaintenanceConfig MaintenanceConfig { get; set; }
    public ConfigDatabase ConfigDatabase { get; set; }

    public async Task InitializeAsync()
    {
        PrivateAddress = NetworkIp.GetPrivateAddress();
        PublicAddress = UsePublicAddress ? await NetworkIp.GetPublicIpAsync() : PrivateAddress;
    }

    public string GetAddess() => PublicAddress;
}
