namespace Common;

[MemoryPack.MemoryPackable]
public partial struct NetworkTerm
{
    public ETermsType Kind { get; set; }
    public string Url { get; set; }
    public uint Version { get; set; }
};

[MemoryPack.MemoryPackable]
public partial struct NetworkAgreeTerms
{
    public EPublisher Publisher { get; set; }
    public ETermsType Kind { get; set; }
    public uint Version { get; set; }
};