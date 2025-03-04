namespace Common;

public class spUpdateUseSerialCoupon(string serialCode, UseCouponLogDao useCouponLog) : BaseStoredProcedure
{
    protected override bool Query()
    {
        try
        {
            var serialCouponCodeCollection = GetCollection<SerialCouponCodeDao>();
            var useCouponLogCollection = GetCollection<UseCouponLogDao>();

            if (useCouponLogCollection.Find(UseCouponLogDao.CouponCodeFilterDefinition(useCouponLog.CouponCodeId)).FirstOrDefault() != null)
            {
                return false;
            }

            useCouponLogCollection.InsertOne(useCouponLog);
            serialCouponCodeCollection.UpdateOne(SerialCouponCodeDao.FilterDefinition(serialCode), SerialCouponCodeDao.UpdateDefinition(true));

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
            var serialCouponCodeCollection = GetCollection<SerialCouponCodeDao>();
            var useCouponLogCollection = GetCollection<UseCouponLogDao>();

            if (await(await useCouponLogCollection.FindAsync(UseCouponLogDao.CouponCodeFilterDefinition(useCouponLog.CouponCodeId))).FirstOrDefaultAsync() != null)
            {
                return false;
            }

            await useCouponLogCollection.InsertOneAsync(useCouponLog);
            await serialCouponCodeCollection.UpdateOneAsync(SerialCouponCodeDao.FilterDefinition(serialCode), SerialCouponCodeDao.UpdateDefinition(true));

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