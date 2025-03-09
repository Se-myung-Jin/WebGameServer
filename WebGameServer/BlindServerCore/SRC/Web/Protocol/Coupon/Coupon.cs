namespace BlindServerCore.Web;

[WebProtocol]
[MemoryPack.MemoryPackable]
public partial class PKT_WEB_CS_COUPON : IWebPacket
{
    public string CouponCode { get; set; }
    public override bool Validate() => Token?.Validate() == true && string.IsNullOrEmpty(CouponCode) == false;
}

[WebProtocol]
[MemoryPack.MemoryPackable]
public partial class PKT_WEB_SC_COUPON : IWebPacket
{
    public Result Result { get; set; }
}
