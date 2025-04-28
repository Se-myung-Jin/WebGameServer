using MemoryPack;

namespace Common.Database.Dao;

[LogTable(
    tableName: "LogCCU",
    useMonthlyPartition: true,
    singleIndexes: new[] { "LogTime" }
)]
[MemoryPackable]
public partial class LogCCUDao : LogBase
{
    public ushort WorldId { get; set; }
    public int Users { get; set; }
}
