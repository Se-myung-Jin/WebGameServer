namespace Common;

public abstract class BaseStoredProcedure
{
    public bool Run() => Query();

    // 스레드풀을 이용하는 경우에만 사용.
    public async Task<bool> StartPoolAync() => await QueryAsync();
    public void StartPoolPost() => _ = QueryAsync();

    protected abstract bool Query();
    protected abstract Task<bool> QueryAsync();
    protected IMongoCollection<T> GetCollection<T>() where T : BaseDao
    {
        return DBContext.Instance.MongoDb.GetCollection<T>();
    }
};