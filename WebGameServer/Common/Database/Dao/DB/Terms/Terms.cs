namespace Common;

[Table("Maintenance", "terms")]
[TableIndex("OwnerKey")]
public class TermsDao : BaseDao
{
    public EPublisher Publisher { get; set; }
    public ETermsType Kind { get; set; }
    public string Url { get; set; }
    public uint Version { get; set; }
};

public class AgreeTerms
{
    public EPublisher Publisher { get; set; }
    public ETermsType Kind { get; set; }
    public uint Version { get; set; }
    public DateTime UpdateTime { get; set; }
};