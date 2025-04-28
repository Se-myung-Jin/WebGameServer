using MemoryPack;
using System.Reflection;

namespace Common.Database.Dao;

[MemoryPackable]
[MemoryPackUnion(1, typeof(LogAuthDao))]
[MemoryPackUnion(2, typeof(LogCCUDao))]
[MemoryPackUnion(3, typeof(LogKPIMetricsDao))]
[MemoryPackUnion(4, typeof(LogNewAccountDao))]
public abstract partial class LogBase
{
    public DateTime LogTime { get; set; } = DateTime.UtcNow;

    private LogTableAttribute LogTableAttr => this.GetType().GetCustomAttribute<LogTableAttribute>();

    public string GetTableName()
    {
        string baseTableName = LogTableAttr?.TableName;
        bool usePartition = LogTableAttr?.UseMonthlyPartition ?? false;

        if (string.IsNullOrEmpty(baseTableName))
        {
            throw new InvalidOperationException($"TableName is not set for {this.GetType().Name}");
        }

        if (usePartition)
        {
            var suffix = DateTime.UtcNow.ToString("yyyyMM");
            return $"{baseTableName}_{suffix}";
        }

        return baseTableName;
    }
}
