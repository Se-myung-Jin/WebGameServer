namespace Common;

public class MaintenanceConfig
{
    public string Url { get; set; }
    public string Domain { get; set; }
    public bool Enable { get; set; }
};

public class ServiceConfig
{
    public bool UsePublicAddress { get; set; }
    public string PublicAddress { get; private set; }
    public string PrivateAddress { get; private set; }
    public MaintenanceConfig MaintenanceConfig { get; set; }

    public async Task InitializeAsync()
    {
        PrivateAddress = NetworkIp.GetPrivateAddress();
        PublicAddress = UsePublicAddress ? await NetworkIp.GetPublicIpAsync() : PrivateAddress;
    }

    public string GetAddess() => PublicAddress;
}
