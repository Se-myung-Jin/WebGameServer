using MemoryPack;
using System.Runtime.InteropServices.JavaScript;

namespace Common;

[MemoryPack.MemoryPackable]
public partial class AccessToken
{
    public long TimeTick { get; set; }
    public long UserId { get; set; }
    public string Key { get; set; }
    public int ValidateHash { get; set; }
    public bool Validate() => string.IsNullOrWhiteSpace(Key) == false && UserId > 0;
};

[WebProtocol]
[MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Sequential)]
public partial class PKT_WEB_CS_MAINTENANCE : IWebPacket
{
    public string HashKey { get; set; }
    public string SubKey { get; set; }
    public string Languages { get; set; }
    public long BuildNumber { get; set; }

    public override bool Validate()
    {
        if (string.IsNullOrEmpty(HashKey) ||
            string.IsNullOrWhiteSpace(Languages))
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
    public DateTime? StartTIme { get; set; }
    public DateTime? EndTIme { get; set; }
    public long ServerTimeTick { get; set; }
    public string WebServiceUrl { get; set; }
};