namespace Common;

public class spSelectDeletedPlayer : BaseStoredProcedure
{
    public List<DeletedPlayerDao> deletedPlayers;

    protected override bool Query()
    {
        try
        {
            var collection = GetCollection<DeletedPlayerDao>();
            deletedPlayers = collection.Find(new BsonDocument()).ToList();

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
            var collection = GetCollection<DeletedPlayerDao>();
            deletedPlayers = await (await collection.FindAsync(new BsonDocument())).ToListAsync();

            return true;
        }
        catch (Exception ex)
        {

        }

        return false;
    }
}