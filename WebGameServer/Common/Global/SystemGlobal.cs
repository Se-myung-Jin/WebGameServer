namespace Common;

public class SystemGlobal : Singleton<SystemGlobal>
{
    public void Initialize(string appName)
    {
        InitializeThreadPool();
    }

    public void StartScheduler()
    {
        
    }

    private void InitializeThreadPool()
    {
        ThreadPool.SetMinThreads(64, 128);
    }
}
