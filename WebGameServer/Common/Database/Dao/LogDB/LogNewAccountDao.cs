using MemoryPack;

namespace Common.Database.Dao;

[LogTable(
    tableName: "LogNewAccount",
    singleIndexes: new[] { "LogTime" }
)]
[MemoryPackable]
public partial class LogNewAccountDao : LogBase
{
    public string AuthId { get; set; }
    public long UserId { get; set; }
    public byte AccountType { get; set; }
    public DateTime CreateTime { get; set; }
}
