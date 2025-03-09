using System;

namespace BlindServerCore.Web;

[MemoryPack.MemoryPackable]
public partial class AccessToken
{
    public long TimeTick { get; set; }
    public long UserId { get; set; }
    public string Key { get; set; }
    public int ValidateHash { get; set; }
    public bool Validate() => string.IsNullOrWhiteSpace(Key) == false && UserId > 0;
}

[MemoryPack.MemoryPackable]
public partial class DeviceData
{
}

[MemoryPack.MemoryPackable]
public partial class BlockDescription
{
    public DateTime EndTime { get; set; }
    public uint Reason { get; set; }
}

[MemoryPack.MemoryPackable]
public partial class ClientServiceOption
{
    public byte WorldRefreshSecond { get; set; } = 3;
    public byte WaitRefreshSecond { get; set; } = 3;
    public byte KeepAliveIntervalSecond { get; set; } = 5;
}

[MemoryPack.MemoryPackable]
public partial struct NetworkTerm
{
    public TermsType Kind { get; set; }
    public string Url { get; set; }
    public uint Version { get; set; }
}

[MemoryPack.MemoryPackable]
public partial struct NetworkAgreeTerms
{
    public Publisher Publisher { get; set; }
    public TermsType Kind { get; set; }
    public uint Version { get; set; }
}