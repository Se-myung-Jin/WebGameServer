namespace Common;

public class spUpdateUseKeywordCoupon(string keyword, long issuedCount, UseCouponLogDao useCouponLog) : BaseStoredProcedure
{
    protected override bool Query()
    {
        try
        {
            var keywordCouponCodeCollection = GetCollection<KeywordCouponCodeDao>();
            var useCouponLogCollection = GetCollection<UseCouponLogDao>();

            useCouponLogCollection.InsertOne(useCouponLog);
            var count = useCouponLogCollection.CountDocuments(UseCouponLogDao.CouponFilterDefinition(useCouponLog.CouponId));

            if (issuedCount < count)
            {
                useCouponLogCollection.DeleteOne(UseCouponLogDao.FilterDefinition(useCouponLog.PlayerId, useCouponLog.CouponId));
                return false;
            }

            keywordCouponCodeCollection.UpdateOne(KeywordCouponCodeDao.FilterDefinition(keyword), KeywordCouponCodeDao.UpdateDefinition(issuedCount - count));


            //TODO: Mail To Reward
            //useCouponLog.PlayerId

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
            var keywordCouponCodeCollection = GetCollection<KeywordCouponCodeDao>();
            var useCouponLogCollection = GetCollection<UseCouponLogDao>();

            await useCouponLogCollection.InsertOneAsync(useCouponLog);
            var count = await useCouponLogCollection.CountDocumentsAsync(UseCouponLogDao.CouponFilterDefinition(useCouponLog.CouponId));

            if (issuedCount < count)
            {
                await useCouponLogCollection.DeleteOneAsync(UseCouponLogDao.FilterDefinition(useCouponLog.PlayerId, useCouponLog.CouponId));
                return false;
            }

            await keywordCouponCodeCollection.UpdateOneAsync(KeywordCouponCodeDao.FilterDefinition(keyword), KeywordCouponCodeDao.UpdateDefinition(issuedCount - count));

            //TODO: Mail To Reward
            //useCouponLog.PlayerId

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}