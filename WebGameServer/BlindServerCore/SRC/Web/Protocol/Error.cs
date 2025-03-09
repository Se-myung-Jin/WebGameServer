namespace BlindServerCore.Web;

[WebProtocol]
[MemoryPack.MemoryPackable]
public partial class PKT_WEB_SC_INTERNALERROR : IWebPacket
{
    public string Message { get; set; }
}