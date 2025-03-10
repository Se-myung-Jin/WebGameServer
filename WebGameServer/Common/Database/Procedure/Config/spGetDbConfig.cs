namespace Common.Database.Procedure;

public class spGetDbConfig : BaseStoredProcedure
{
    public DatabaseConfigDao Result;

    protected override bool Query()
    {
        try
        {
            var collection = GetCollection<DatabaseConfigDao>();
            Result = collection.Find(x=>true).FirstOrDefault();
            
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
            var collection = GetCollection<DatabaseConfigDao>();
            Result = await (await collection.FindAsync(x=>true)).FirstOrDefaultAsync();
            
            return true;
        }
        catch (Exception ex)
        {

        }
        return false;
    }
}
