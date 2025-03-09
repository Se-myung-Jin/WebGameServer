using System;
using System.Collections.Generic;

namespace BlindServerCore.Web;

[WebProtocol]
[MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Sequential)]
public partial class PKT_WEB_CS_MAINTENANCE : IWebPacket
{
    public string HashKey { get; set; }
    public string SubKey { get; set; }
    public string Languages { get; set; }
    public OsType OSType { get; set; }
    public Publisher Publisher { get; set; }
    public long BuildNumber { get; set; }

    public override bool Validate()
    {
        if (string.IsNullOrEmpty(HashKey) || string.IsNullOrWhiteSpace(Languages))
        {
            return false;
        }

        return true;
    }
};

[WebProtocol]
[MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Sequential)]
public partial class PKT_WEB_SC_MAINTENANCE : IWebPacket
{
    public Result Result { get; set; }
    public DateTime? StartTIme { get; set; }
    public DateTime? EndTIme { get; set; }
    public long ServerTimeTick { get; set; }
    public List<NetworkTerm> PolicyList { get; set; }
    public ClientServiceOption ServiceOption { get; set; }
    public string WebServiceUrl { get; set; }
    public MaintenanceUpdateType UpdateType { get; set; }
};