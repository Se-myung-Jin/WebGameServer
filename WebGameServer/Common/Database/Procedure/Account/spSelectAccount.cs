namespace Common.Database.Procedure;

public class spSelectAccount(AuthType type, string authId, string token) : BaseStoredProcedure
{
    public AccountDao OutResult { get; set; }

    protected override bool Query()
    {
        try
        {
            var collection = GetCollection<AccountDao>();
            var findAccount = collection.Find(AccountDao.CreateFilter((byte)type, authId)).FirstOrDefault();
            if (findAccount != null && findAccount.Token.Equals(token))
            {
                OutResult = findAccount;
            }

            return true;
        }
        catch (Exception ex)
        {

        }

        return false;

    }

    protected override async Task<bool> QueryAsync()
    {
        try
        {
            var collection = GetCollection<AccountDao>();
            var findAccount = await (await collection.FindAsync(AccountDao.CreateFilter((byte)type, authId))).FirstOrDefaultAsync();
            if (findAccount != null && findAccount.Token.Equals(token))
            {
                OutResult = findAccount;
            }

            return true;
        }
        catch (Exception ex)
        {

        }

        return false;
    }
}
