using Common;

[Table("Servers", "KeywordCouponCodes")]
[TableIndex("Keyword", unique: true)]
public class KeywordCouponCodeDao : BaseDao
{
    public string Keyword { get; set; }
    public ObjectId CouponId { get; set; }
    public long RemainCount { get; set; }

    public static FilterDefinition<KeywordCouponCodeDao> FilterDefinition(string keyword)
    {
        FilterDefinitionBuilder<KeywordCouponCodeDao> builder = new();
        return builder.Eq("Keyword", keyword);
    }

    public static UpdateDefinition<KeywordCouponCodeDao> UpdateDefinition(long remainCount)
    {
        UpdateDefinitionBuilder<KeywordCouponCodeDao> builder = new();
        return builder.Set("RemainCount", remainCount);
    }
}