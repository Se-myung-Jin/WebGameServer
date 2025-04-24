using MemoryPack;

namespace Common.Database.Dao;

[LogTable(
    tableName: "LogAuth",
    useMonthlyPartition: true,
    singleIndexes: new[] { "UserId", "LogTime" } 
//compositeIndexes: new[] { "UserId, ItemId", "ItemId,ObtainedAt" }, // 복합 인덱스 2개
//uniqueIndexes: new[] { "UserId, ItemId, ObtainedAt", "UserId, ObtainedAt" } // 유니크 인덱스 2개
)]
[MemoryPackable]
public partial class LogAuthDao : LogBase
{
    public long UserId { get; set; }
    public byte AccountType { get; set; }
    public byte Publisher { get; set; }
    public string IP { get; set; }
    public string ClientVersion { get; set; }
    public string DeviceInfo { get; set; }
    public string Platform { get; set; }
}
// DAU, MAU, WAU