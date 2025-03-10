namespace Common.Database.Procedure;

public class spUpdateServerStatus(ServerStatusDao data) : BaseStoredProcedure
{
    private ReplaceOptions _upsertOption = new ReplaceOptions { IsUpsert = true };
    
    protected override bool Query()
    {
        try
        {
            data.UpdateTime = DateTime.UtcNow;
            var collection = DBContext.Instance.MongoDb.GetCollection<ServerStatusDao>();
            
            if (data._id.Equals(ObjectId.Empty))
            {
                collection.InsertOne(data);
                
                return true;
            }

            collection.ReplaceOne(new BsonDocument("_id", data._id), data, _upsertOption);
            
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
            var collection = DBContext.Instance.MongoDb.GetCollection<ServerStatusDao>();
            
            if (data._id.Equals(ObjectId.Empty))
            {
                await collection.InsertOneAsync(data);
                
                return true;
            }

            await collection.ReplaceOneAsync(new BsonDocument("_id", data._id), data, _upsertOption);
            
            return true;
        }
        catch (Exception ex)
        {

        }
        return false;
    }
}
