
namespace Common;

public class spSelectAllCouponData : BaseStoredProcedure
{
    public List<CouponDao> CouponList { get; set; }
    public List<KeywordCouponCodeDao> KeywordCouponCodeList { get; set; }

    protected override bool Query()
    {
        try
        {
            var couponInfoCollection = GetCollection<CouponDao>();
            var KeywordCouponCodeCollection = GetCollection<KeywordCouponCodeDao>();

            CouponList = couponInfoCollection.Find(new BsonDocument()).ToList();
            KeywordCouponCodeList = KeywordCouponCodeCollection.Find(new BsonDocument()).ToList();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    protected override async Task<bool> QueryAsync()
    {
        try
        {
            var couponInfoCollection = GetCollection<CouponDao>();
            var KeywordCouponCodeCollection = GetCollection<KeywordCouponCodeDao>();

            CouponList = await (await couponInfoCollection.FindAsync(new BsonDocument())).ToListAsync();
            KeywordCouponCodeList = await (await KeywordCouponCodeCollection.FindAsync(new BsonDocument())).ToListAsync();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}