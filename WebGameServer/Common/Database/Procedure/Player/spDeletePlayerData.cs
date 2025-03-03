namespace Common;

public class spDeletePlayerData(DeletedPlayerDao deletedPlayer) : BaseStoredProcedure
{
    protected override bool Query()
    {
        try
        {
            var playerCollection = DBContext.Instance.MongoDb.GetCollection<PlayerDao>();
            var player = playerCollection.FindOneAndDelete(new BsonDocument("_id", deletedPlayer.PlayerId));
            if (player == null)
            {
                return false;
            }

            var OwnerKey = player._id;

            var deletedPlayerCollection = DBContext.Instance.MongoDb.GetCollection<DeletedPlayerDao>();
            deletedPlayerCollection.DeleteOne(new BsonDocument("_id", deletedPlayer._id));

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
            var playerCollection = DBContext.Instance.MongoDb.GetCollection<PlayerDao>();
            var player = await playerCollection.FindOneAndDeleteAsync(new BsonDocument("_id", deletedPlayer.PlayerId));
            if (player == null)
            {
                return false;
            }

            var OwnerKey = player._id;

            var deletedPlayerCollection = DBContext.Instance.MongoDb.GetCollection<DeletedPlayerDao>();
            await deletedPlayerCollection.DeleteOneAsync(new BsonDocument("_id", deletedPlayer._id));

            return true;
        }
        catch (Exception ex)
        {

        }

        return false;
    }
}