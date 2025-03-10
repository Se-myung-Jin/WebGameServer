namespace Common.Database.Procedure;

public class spInsertAccount(AccountDao data) : BaseStoredProcedure
{
    protected override bool Query()
    {
        try
        {
            var collection = GetCollection<AccountDao>();
            collection.InsertOne(data);

            return true;
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }

        return false;

    }

    protected override async Task<bool> QueryAsync()
    {
        try
        {
            var collection = GetCollection<AccountDao>();
            await collection.InsertOneAsync(data);

            return true;
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }

        return false;
    }
}