namespace MaintenanceServer;

public partial class RestApiRouter
{
    [ApiHandleAttribute]
    private async Task<IWebPacket> Maintenance(string clientAddress, IWebPacket protocol)
    {
        var pkt = protocol as PKT_WEB_CS_MAINTENANCE;
        var response = new PKT_WEB_SC_MAINTENANCE();

        if (pkt.Validate() == false)
        {
            return response;
        }

        response.ServerTimeTick = DateTime.UtcNow.Ticks;
        response.StartTIme = new DateTime(2024, 1, 1);
        response.EndTIme = new DateTime(2099, 1, 1);

        return response;
    }
}