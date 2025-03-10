namespace MaintenanceServer;

public partial class RestApiRouter
{
    [ApiHandleAttribute]
    private async Task<IWebPacket> Coupon(string clientAddress, IWebPacket protocol)
    {
        var pkt = protocol as PKT_WEB_CS_COUPON;
        var response = new PKT_WEB_SC_COUPON();
        response.Result = Result.Success;

        if (pkt.Validate() == false)
        {
            response.Result = Result.Error_InvalidParameter;
            return response;
        }

        var validateResult = await ProxyCommand.Instance.Redis.ValidateKeyAccountAsync(pkt.Token);
        if (validateResult.result != Result.Success)
        {
            response.Result = validateResult.result;
            return response;
        }

        //TEST_CODE

        //TODO: Create Serial Coupon
        //CouponDao keywordTestCouponDao = new();
        //keywordTestCouponDao.Description = "Keyword Test Coupon";
        //keywordTestCouponDao.CreateTime = TimeUtils.GetTime();
        //keywordTestCouponDao.ExpireTime = TimeUtils.GetTime().AddMinutes(15);
        //keywordTestCouponDao.UseCountPerUser = 1;
        //keywordTestCouponDao.IssuedCount = 10;
        //keywordTestCouponDao.RewardItemInfoList = null;
        //await CouponManager.Instance.IssueKeywordCouponAsync(pkt.CouponCode, keywordTestCouponDao);

        //TODO: Create Keyword Coupon
        //CouponDao serialTestCouponDao = new();
        //serialTestCouponDao.Description = "Serial Test Coupon";
        //serialTestCouponDao.CreateTime = TimeUtils.GetTime();
        //serialTestCouponDao.ExpireTime = TimeUtils.GetTime().AddMinutes(15);
        //serialTestCouponDao.UseCountPerUser = 3;
        //serialTestCouponDao.IssuedCount = 10;
        //serialTestCouponDao.RewardItemInfoList = null;
        //await CouponManager.Instance.IssueSerialCouponAsync(serialTestCouponDao);

        var playerId = validateResult.account.Token.UserId;

        response.Result = await CouponManager.Instance.UseCoupon(pkt.CouponCode, playerId);
        return response;
    }
}
