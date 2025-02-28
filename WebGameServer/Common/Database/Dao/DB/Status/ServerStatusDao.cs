namespace Common;

public class ServerStatusDao
{
    public EServerType ServerType { get; set; }
    public string PublicAddress { get; set; }
    public string PrivateAddress { get; set; }
    public uint ServerId { get; set; }
    public ushort Port { get; set; }
    public float HighMemoryGb { get; set; }
    public float MemoryGb { get; set; }
    public float HighCpuPercent { get; set; }
    public float CpuPercent { get; set; }
    public DateTime UpdateTime { get; set; }
    public string CommitHash { get; set; }
}
