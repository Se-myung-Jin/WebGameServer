using MemoryPack;

namespace Common.Database.Dao;

[LogTable(
    tableName: "LogItemGet",
    useMonthlyPartition: true
    //singleIndexes: new[] { "UserId", "ItemId" }, // 단일 컬럼 인덱스 2개
    //compositeIndexes: new[] { "UserId, ItemId", "ItemId,ObtainedAt" }, // 복합 인덱스 2개
    //uniqueIndexes: new[] { "UserId, ItemId, ObtainedAt", "UserId, ObtainedAt" } // 유니크 인덱스 2개
)]
[MemoryPackable]
public partial class LogItemGetDao : LogBase
{
    public long UserId { get; set; } // INDEX(UserId)
    public int ItemId { get; set; } // INDEX(ItemId)
    public int Quantity { get; set; } // 일반 컬럼
    public DateTime ObtainedAt { get; set; } // 복합 인덱스 포함됨
}

[LogTable("LogItemRemove")]
[MemoryPackable]
public partial class LogItemRemoveDao : LogBase
{
    public int Id { get; set; }
    public string Message { get; set; }
    public string LogLevel { get; set; }
    public DateTime RemovedAt { get; set; }
}