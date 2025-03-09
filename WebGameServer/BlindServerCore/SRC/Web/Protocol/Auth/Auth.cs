using System.Collections.Generic;

namespace BlindServerCore.Web;

[WebProtocol]
[MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Sequential)]
public partial class PKT_WEB_CS_AUTH : IWebPacket
{
    public AuthType AuthType { get; set; }
    public string AuthId { get; set; }
    public string AuthToken { get; set; }
    public DeviceData DeviceInfo { get; set; }
    public List<NetworkAgreeTerms> AgreeTermsList { get; set; }
    public Publisher Publisher { get; set; }
    public override bool Validate() => AuthType > AuthType.UnDefine && AuthType < AuthType.Max;
};

[WebProtocol]
[MemoryPack.MemoryPackable(MemoryPack.GenerateType.VersionTolerant, MemoryPack.SerializeLayout.Sequential)]
public partial class PKT_WEB_SC_AUTH : IWebPacket
{
    public Result Result { get; set; }
    /// <summary>
    /// 게스트 가입된 경우에만 해당 데이터가 설정 됩니다.
    /// </summary>
    public string AuthId { get; set; }
    /// <summary>
    /// 게스트 가입된 경우에만 해당 데이터가 설정 됩니다.
    /// </summary>
    public string AuthToken { get; set; }
    public bool IsCreate { get; set; }
    /// <summary>
    /// 블럭당한 경우 블럭 정보
    /// </summary>
    public BlockDescription Block { get; set; }
};