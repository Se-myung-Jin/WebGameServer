namespace Common.Database.Procedure;

public class spCountUseCouponLog(long PlayerId, ObjectId CouponId) : BaseStoredProcedure
{
    public long UseCount { get; set; }

    protected override bool Query()
    {
        try
        {
            var collection = GetCollection<UseCouponLogDao>();
            UseCount = collection.CountDocuments(UseCouponLogDao.FilterDefinition(PlayerId, CouponId));

            return true;
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);

            return false;
        }
    }

    protected override async Task<bool> QueryAsync()
    {
        try
        {
            var collection = GetCollection<UseCouponLogDao>();
            UseCount = await collection.CountDocumentsAsync(UseCouponLogDao.FilterDefinition(PlayerId, CouponId));

            return true;
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);

            return false;
        }
    }
}