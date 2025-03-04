namespace Common;

[Table("Accounts", "account")]
[TableIndex("AccountType", "AuthId")]
[TableIndex("UserId")]
public class AccountDao : BaseDao
{
    public string AuthId { get; set; }
    public string Token { get; set; }
    public long UserId { get; set; }
    public byte AccountType { get; set; }
    public byte Grade { get; set; }
    public int BlockReason { get; set; }
    public ushort LastSelectWorld { get; set; }
    public DateTime BlockTIme { get; set; }
    public DateTime LastLoginTime { get; set; }
    public DateTime CreateTime { get; set; }
    public List<AgreeTerms> AgreeTermsList { get; set; }

    public (bool confirm, bool isUpdate) CheckTerms(List<NetworkTerm> termsList, List<NetworkAgreeTerms> agreeList, DateTime updateTime)
    {
        if (termsList == null)
        {
            return default;
        }

        bool isUpdate = false;
        for (int i = 0, count = termsList.Count; i < count; ++i)
        {
            var serverTerms = termsList[i];
            var findAgreeTerms = agreeList.Find(x => x.Kind == serverTerms.Kind && x.Version == serverTerms.Version);
            if (findAgreeTerms.Version == 0)
            {
                return (false, false);
            }

            var oldAgree = AgreeTermsList?.Find(x => x.Kind == serverTerms.Kind && x.Version == serverTerms.Version);
            if (oldAgree == null)
            {
                isUpdate = true;
                AgreeTermsList ??= new();
                AgreeTermsList.Add(new AgreeTerms { Kind = serverTerms.Kind, Publisher = findAgreeTerms.Publisher, Version = serverTerms.Version, UpdateTime = updateTime });
                continue;
            }
        }

        return (true, isUpdate);
    }

    public static FilterDefinition<AccountDao> CreateFilter(byte accountType, string authId)
    {
        var builder = new FilterDefinitionBuilder<AccountDao>();
        return builder.Eq("AccountType", accountType) & builder.Eq("AuthId", authId);
    }

    public static FilterDefinition<AccountDao> CreateFilter(long userId)
    {
        return new FilterDefinitionBuilder<AccountDao>().Eq("UserId", userId);
    }

    public static UpdateDefinition<AccountDao> UpdateLastLoginTime(DateTime lastLoginTime)
    {
        return new UpdateDefinitionBuilder<AccountDao>().Set("LastLoginTime", lastLoginTime);
    }

    public static UpdateDefinition<AccountDao> UpdateBlockTime(int blockReason, DateTime blockTIme)
    {
        return new UpdateDefinitionBuilder<AccountDao>().Set("BlockReason", blockReason).Set("BlockTIme", blockTIme);
    }

    public static UpdateDefinition<AccountDao> UpdateLastSelectWorld(ushort lastSelectWorld)
    {
        return new UpdateDefinitionBuilder<AccountDao>().Set("LastSelectWorld", lastSelectWorld);
    }
};
