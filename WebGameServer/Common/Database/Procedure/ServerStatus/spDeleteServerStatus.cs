namespace Common.Database.Procedure;

public class spDeleteServerStatus(ServerStatusDao data) : BaseStoredProcedure
{
    protected override bool Query()
    {
        try
        {
            data.UpdateTime = DateTime.UtcNow;
            var collection = GetCollection<ServerStatusDao>();
            
            collection.DeleteMany(new BsonDocument("PrivateAddress", data.PrivateAddress));
            
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
            data.UpdateTime = DateTime.UtcNow;
            var collection = GetCollection<ServerStatusDao>();
            
            await collection.DeleteManyAsync(new BsonDocument("PrivateAddress", data.PrivateAddress));
            
            return true;
        }
        catch (Exception ex)
        {

        }
        return false;
    }
}
