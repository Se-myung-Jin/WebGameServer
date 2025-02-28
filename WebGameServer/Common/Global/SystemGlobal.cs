namespace Common;

public class SystemGlobal : Singleton<SystemGlobal>
{
    private ScheduleController _mainScheduler = new ScheduleController();

    public void Initialize(string appName, byte jobThreadCount = 4, byte dbThreadCount = 8)
    {
        InitializeThreadPool();
        InitScheduler(jobThreadCount, dbThreadCount);
    }

    public void StartScheduler()
    {
        _mainScheduler.Start();
    }

    private void InitializeThreadPool()
    {
        ThreadPool.SetMinThreads(64, 128);
    }

    private void InitScheduler(byte jobThreadCount, byte dbThreadCount)
    {
        var timeScheduler = new TimeScheduler();

        _mainScheduler.Initialize(jobThreadCount);
        _mainScheduler.Add(timeScheduler);

        TimeJob.OwnerScheduler = timeScheduler;
    }
}
