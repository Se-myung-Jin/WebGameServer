#pragma warning disable CS1998

namespace GameServer;

public partial class RestApiRouter
{
    [ApiHandleAttribute]
    private async Task<IWebPacket> Hello(string clientAddress, IWebPacket protocol)
    {
        var pkt = protocol as PKT_WEB_CS_HELLO;
        return new PKT_WEB_SC_HELLO { Response = $"{BlindServerCore.Utils.TimeUtils.GetTime().ConvertTimeString()} Message: {pkt.Hello}" };
    }
}
