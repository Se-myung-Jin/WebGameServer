namespace Common;

public class spInsertKeywordCoupon(CouponDao Coupon, KeywordCouponCodeDao KeywordCouponCode) : BaseStoredProcedure
{
    public EResult Error { get; set; } = EResult.Success;

    protected override bool Query()
    {
        try
        {
            if (KeywordCouponCode.Keyword.Length == SerialCouponCodeUtils.KeywordLength)
            {
                var serialCouponCodeCollection = GetCollection<SerialCouponCodeDao>();
                if (serialCouponCodeCollection.Find(SerialCouponCodeDao.FilterDefinition(KeywordCouponCode.Keyword)).FirstOrDefault() != null)
                {
                    Error = EResult.Error_DuplicateName;

                    return true;
                }
            }

            var couponCollection = GetCollection<CouponDao>();

            Coupon._id = ObjectId.GenerateNewId();
            if (InsertKeywordCouponCode(Coupon._id) == false)
            {
                Error = EResult.Error_DuplicateName;

                return true;
            }

            couponCollection.InsertOne(Coupon);

            return true;
        }
        catch (Exception ex)
        {
            Error = EResult.Error_Internal;

            return false;
        }
    }

    protected override async Task<bool> QueryAsync()
    {
        try
        {
            if (KeywordCouponCode.Keyword.Length == SerialCouponCodeUtils.KeywordLength)
            {
                var serialCouponCodeCollection = GetCollection<SerialCouponCodeDao>();
                if (await (await serialCouponCodeCollection.FindAsync(SerialCouponCodeDao.FilterDefinition(KeywordCouponCode.Keyword))).FirstOrDefaultAsync() != null)
                {
                    Error = EResult.Error_DuplicateName;

                    return true;
                }
            }

            var couponCollection = GetCollection<CouponDao>();

            Coupon._id = ObjectId.GenerateNewId();
            if (await InsertKeywordCouponCodeAsync(Coupon._id) == false)
            {
                Error = EResult.Error_DuplicateName;

                return true;
            }

            await couponCollection.InsertOneAsync(Coupon);

            return true;
        }
        catch (Exception ex)
        {
            Error = EResult.Error_Internal;

            return false;
        }
    }

    private bool InsertKeywordCouponCode(ObjectId couponId)
    {
        var KeywordCouponCodeCollection = GetCollection<KeywordCouponCodeDao>();

        try
        {
            KeywordCouponCode.CouponId = couponId;
            KeywordCouponCodeCollection.InsertOne(KeywordCouponCode);

            return true;
        }
        catch (MongoWriteException ex)
        {
            if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return false;
            }
        }

        return false;
    }

    private async Task<bool> InsertKeywordCouponCodeAsync(ObjectId couponId)
    {
        var KeywordCouponCodeCollection = GetCollection<KeywordCouponCodeDao>();

        try
        {
            KeywordCouponCode.CouponId = couponId;
            await KeywordCouponCodeCollection.InsertOneAsync(KeywordCouponCode);

            return true;
        }
        catch (MongoWriteException ex)
        {
            if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return false;
            }
        }

        return false;
    }
}