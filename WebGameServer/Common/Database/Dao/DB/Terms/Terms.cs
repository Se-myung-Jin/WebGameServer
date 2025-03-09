namespace Common.Database.Dao;

[Table("Maintenance", "terms")]
[TableIndex("OwnerKey")]
public class TermsDao : BaseDao
{
    public Publisher Publisher { get; set; }
    public TermsType Kind { get; set; }
    public string Url { get; set; }
    public uint Version { get; set; }
};

public class AgreeTerms
{
    public Publisher Publisher { get; set; }
    public TermsType Kind { get; set; }
    public uint Version { get; set; }
    public DateTime UpdateTime { get; set; }
};