namespace Common.Database.Dao;

[Table("Servers", "UseCouponLog")]
[TableIndex("PlayerId", "CouponId")]
[TableIndex("CouponId")]
[TableIndex("CouponCodeId")]
public class UseCouponLogDao : BaseDao
{
    public long PlayerId { get; set; }
    public ObjectId CouponId { get; set; }
    public ObjectId CouponCodeId { get; set; }
    public string CouponCode { get; set; }
    public DateTime LogTime { get; set; }

    public static FilterDefinition<UseCouponLogDao> FilterDefinition(long playerId, ObjectId couponId)
    {
        FilterDefinitionBuilder<UseCouponLogDao> builder = new();

        return builder.Eq("PlayerId", playerId) & builder.Eq("CouponId", couponId);
    }
    public static FilterDefinition<UseCouponLogDao> FilterDefinition(long playerId)
    {
        FilterDefinitionBuilder<UseCouponLogDao> builder = new();

        return builder.Eq("PlayerId", playerId);
    }
    public static FilterDefinition<UseCouponLogDao> CouponFilterDefinition(ObjectId couponId)
    {
        FilterDefinitionBuilder<UseCouponLogDao> builder = new();

        return builder.Eq("CouponId", couponId);
    }
    public static FilterDefinition<UseCouponLogDao> CouponCodeFilterDefinition(ObjectId ouponCodeId)
    {
        FilterDefinitionBuilder<UseCouponLogDao> builder = new();

        return builder.Eq("CouponCodeId", ouponCodeId);
    }
}