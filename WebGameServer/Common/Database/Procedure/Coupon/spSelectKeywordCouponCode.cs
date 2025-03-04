namespace Common;

public class spSelectKeywordCouponCode(string keyword) : BaseStoredProcedure
{
    public KeywordCouponCodeDao KeywordCouponCode { get; set; }

    protected override bool Query()
    {
        try
        {
            var collection = GetCollection<KeywordCouponCodeDao>();
            KeywordCouponCode = collection.Find(KeywordCouponCodeDao.FilterDefinition(keyword)).FirstOrDefault();

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
            var collection = GetCollection<KeywordCouponCodeDao>();
            KeywordCouponCode = await (await collection.FindAsync(KeywordCouponCodeDao.FilterDefinition(keyword))).FirstOrDefaultAsync();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}