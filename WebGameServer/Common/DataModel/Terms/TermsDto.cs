namespace Common;

[MemoryPack.MemoryPackable]
public partial struct NetworkTerm
{
    public TermsType Kind { get; set; }
    public string Url { get; set; }
    public uint Version { get; set; }
};

[MemoryPack.MemoryPackable]
public partial struct NetworkAgreeTerms
{
    public Publisher Publisher { get; set; }
    public TermsType Kind { get; set; }
    public uint Version { get; set; }
};