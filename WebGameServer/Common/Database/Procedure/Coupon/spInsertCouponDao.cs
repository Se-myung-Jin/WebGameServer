namespace Common.Database.Procedure;

public class spInsertCouponDao(CouponDao Coupon) : BaseStoredProcedure
{
    protected override bool Query()
    {
        try
        {
            var couponCollection = GetCollection<CouponDao>();
            couponCollection.InsertOne(Coupon);

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
            var couponCollection = GetCollection<CouponDao>();
            await couponCollection.InsertOneAsync(Coupon);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}