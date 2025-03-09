namespace BlindServerCore.Web;

[WebProtocol]
[MemoryPack.MemoryPackable]
public partial class PKT_WEB_CS_HELLO : IWebPacket
{
    public string Hello { get; set; }
};

[WebProtocol]
[MemoryPack.MemoryPackable]
public partial class PKT_WEB_SC_HELLO : IWebPacket
{
    public string Response {  get; set; }
};
