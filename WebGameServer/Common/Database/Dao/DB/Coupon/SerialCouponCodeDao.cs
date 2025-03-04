using Common;

[Table("Servers", "SerialCouponCodes")]
[TableIndex("SerialCode", unique: true)]
[TableIndex("CouponId")]
public class SerialCouponCodeDao : BaseDao
{
    public string SerialCode { get; set; }
    public ObjectId CouponId { get; set; }
    public long PlayerId { get; set; }
    public bool IsUsed { get; set; }

    public static FilterDefinition<SerialCouponCodeDao> FilterDefinition(string serialCode)
    {
        FilterDefinitionBuilder<SerialCouponCodeDao> builder = new();
        return builder.Eq("SerialCode", serialCode);
    }

    public static FilterDefinition<SerialCouponCodeDao> FilterDefinition(ObjectId couponId)
    {
        FilterDefinitionBuilder<SerialCouponCodeDao> builder = new();
        return builder.Eq("CouponId", couponId);
    }

    public static UpdateDefinition<SerialCouponCodeDao> UpdateDefinition(bool isUsed)
    {
        UpdateDefinitionBuilder<SerialCouponCodeDao> builder = new();
        return builder.Set("IsUsed", isUsed);
    }
}