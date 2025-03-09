using BlindServerCore.Log;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace BlindServerCore.Threads;

public class TimeScheduler : JobScheduler
{
    private ConcurrentDictionary<long, TimeJob> _timeJobById = new ConcurrentDictionary<long, TimeJob>();
    private long _id = 0;

    protected override void OnProcess(long slice)
    {
        var currentTick = DateTime.UtcNow.Ticks;
        foreach (var element in _timeJobById)
        {
            var job = element.Value;
            if (job.IsTimeOver(currentTick))
            {
                _elapseTimer.Start();

                try
                {
                    job.Execute();
                    if (job.ExecutePostProcess() == false)
                    {
                        _timeJobById.TryRemove(element.Key, out var removeItem);
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Log.Error(ex);
                }

                _elapseTimer.Stop();
                if (_elapseTimer.ElapsedMilliseconds >= 300)
                {

                }
            }
        }
    }

    public void Add(TimeJob job)
    {
        var value = Interlocked.Increment(ref _id);

        _timeJobById.TryAdd(value, job);
    }
}
