namespace Common.Database.Dao;

public abstract class BaseDao
{
    public ObjectId _id { get; set; }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static BsonDocument GetOwnerKeyFilter(ObjectId ownerKey)
    {
        return new BsonDocument("OwnerKey", ownerKey);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static BsonDocument GetDBKeyFilter(ObjectId key)
    {
        return new BsonDocument("_id", key);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static FilterDefinition<T> GetWorldKeyFilter<T>(long userId, ushort worldId)
    {
        FilterDefinitionBuilder<T> builder = new();

        return builder.Eq("UserId", userId) & builder.Eq("WorldId", worldId);
    }

    public bool IsNewData() => _id.Equals(MongoDB.Bson.ObjectId.Empty);
}

public abstract partial class BasePlayerDataDao : BaseDao
{
    public ObjectId OwnerKey { get; set; }
    public long PlayerId { get; set; }
}