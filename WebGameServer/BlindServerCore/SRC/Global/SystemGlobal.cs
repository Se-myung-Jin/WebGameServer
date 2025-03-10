using BlindServerCore.Log;
using BlindServerCore.Threads;
using BlindServerCore.Utils;
using System.Threading;

namespace BlindServerCore;

public class SystemGlobal : Singleton<SystemGlobal>
{
    public Microsoft.IO.RecyclableMemoryStreamManager RecycleMemory { get; private set; }

    private ScheduleController _mainScheduler = new ScheduleController();

    public void Initialize(string appName, byte jobThreadCount = 4, byte dbThreadCount = 8)
    {
        LogSystem.Initialize(appName);

        InitializeThreadPool();
        InitScheduler(jobThreadCount, dbThreadCount);
        InitializeRecycleMemory();
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

    private void InitializeRecycleMemory()
    {
        var options = new Microsoft.IO.RecyclableMemoryStreamManager.Options();
        options.BlockSize = 1024;
        options.LargeBufferMultiple = 1024 * 1024;
        options.MaximumBufferSize = options.LargeBufferMultiple * 16;
        options.AggressiveBufferReturn = true;
        options.GenerateCallStacks = true;
        options.MaximumLargePoolFreeBytes = options.MaximumBufferSize * 4;
        options.MaximumSmallPoolFreeBytes = options.BlockSize * 250;
        RecycleMemory = new Microsoft.IO.RecyclableMemoryStreamManager(options);
    }
}
