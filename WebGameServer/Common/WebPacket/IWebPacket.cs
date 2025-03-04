namespace Common;

public class WebProtocolAttribute : Attribute
{
}

[MemoryPack.MemoryPackable]
[MemoryPack.MemoryPackUnion(1, typeof(PKT_WEB_CS_MAINTENANCE))]
[MemoryPack.MemoryPackUnion(2, typeof(PKT_WEB_SC_MAINTENANCE))]
public abstract partial class IWebPacket
{
    public AccessToken Token { get; set; }

    public virtual bool Validate() => false;
};