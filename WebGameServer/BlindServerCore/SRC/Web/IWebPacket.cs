using System;

namespace BlindServerCore.Web;

public class WebProtocolAttribute : Attribute
{
    
}

[MemoryPack.MemoryPackable]
[MemoryPack.MemoryPackUnion(1, typeof(PKT_WEB_CS_MAINTENANCE))]
[MemoryPack.MemoryPackUnion(2, typeof(PKT_WEB_SC_MAINTENANCE))]
[MemoryPack.MemoryPackUnion(3, typeof(PKT_WEB_CS_AUTH))]
[MemoryPack.MemoryPackUnion(4, typeof(PKT_WEB_SC_AUTH))]
[MemoryPack.MemoryPackUnion(5, typeof(PKT_WEB_CS_HELLO))]
[MemoryPack.MemoryPackUnion(6, typeof(PKT_WEB_SC_HELLO))]
[MemoryPack.MemoryPackUnion(7, typeof(PKT_WEB_SC_COUPON))]
[MemoryPack.MemoryPackUnion(8, typeof(PKT_WEB_CS_COUPON))]
public abstract partial class IWebPacket
{
    public AccessToken Token { get; set; }

    public virtual bool Validate() => false;
}