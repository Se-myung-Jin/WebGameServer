using MemoryPack;

namespace Common.Database.Dao;

[LogTable(
    tableName: "LogKPIMetrics",
    singleIndexes: new[] { "LogTime" }
)]
[MemoryPackable]
public partial class LogKPIMetricsDao : LogBase
{
    public uint Dau { get; set; }
    public uint Wau { get; set; }
    public uint Mau { get; set; }
    public uint NewUsers { get; set; }
    public float Retention_1 { get; set; }
    public float Retention_7 { get; set; }
    public float Retention_14 { get; set; }
    public float Retention_30 { get; set; }
}
