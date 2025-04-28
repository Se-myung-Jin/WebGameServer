namespace Common;

public class GameConfig
{
    public string Url { get; set; }
    public string Domain { get; set; }
    public bool Enable { get; set; }
}

public class MaintenanceConfig
{
    public string Url { get; set; }
    public string Domain { get; set; }
    public bool Enable { get; set; }
}

public class LogAggregationConfig
{
    public string Url { get; set; }
    public string Domain { get; set; }
    public bool Enable { get; set; }
}

public class LogAnalyticsConfig
{
    public string Url { get; set; }
    public string Domain { get; set; }
    public bool Enable { get; set; }
}

public class ConfigDatabase
{
    public string Name { get; set; }
    public string Server { get; set; }
    public string Category { get; set; }
    public string ReplicaType { get; set; }

    public DBConfig DbPoolConfig()
    {
        Enum.TryParse<DataBaseCategory>(Category, out var selectCategory);
        Enum.TryParse<ReplicaType>(ReplicaType, out var selectReplica);

        return new DBConfig(Name, selectCategory, selectReplica, Server, 0);
    }
}

public class ConfigRedis
{
    public ushort DbId { get; set; }
    public string Name { get; set; }
    public string Server { get; set; }
    public bool Enable { get; set; }

    public DBConfig DbPoolConfig()
    {
        return new DBConfig(Name, DataBaseCategory.REDIS, ReplicaType.MASTER, Server, DbId);
    }
}

public class ConfigLogDatabase
{
    public string Name { get; set; }
    public string Server { get; set; }
    public string Category { get; set; }
    public string Replica { get; set; }

    public DBConfig DbPoolConfig()
    {
        return new DBConfig(Name, DataBaseCategory.MYSQL, ReplicaType.MASTER, Server, 0);
    }
}

public class ServiceConfig
{
    public bool UsePublicAddress { get; set; }
    public string PublicAddress { get; private set; }
    public string PrivateAddress { get; private set; }
    public GameConfig GameConfig { get; set; }
    public MaintenanceConfig MaintenanceConfig { get; set; }
    public LogAggregationConfig LogAggregationConfig { get; set; }
    public LogAnalyticsConfig LogAnalyticsConfig { get; set; }
    public ConfigDatabase ConfigDatabase { get; set; }

    public async Task InitializeAsync()
    {
        PrivateAddress = NetworkIp.GetPrivateAddress();
        PublicAddress = UsePublicAddress ? await NetworkIp.GetPublicIpAsync() : PrivateAddress;
    }

    public string GetAddess() => PublicAddress;
}
