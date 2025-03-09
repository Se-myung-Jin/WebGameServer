namespace Common.Database.Dao;

[Table("Servers", "Coupon")]
public class CouponDao : BaseDao
{
    public string Description { get; set; }
    public byte UseCountPerUser { get; set; }
    public List<NetworkCodeCount> RewardItemInfoList { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime ExpireTime { get; set; }
    public long IssuedCount { get; set; }
}
