using BlindServerCore.Log;
using System;
using System.Collections.Generic;

namespace BlindServerCore.Threads;

public class ScheduleController : ThreadContainer
{
    private List<BaseScheduler> _schedulerList = new List<BaseScheduler>();
    private List<List<BaseScheduler>> _threadAccessList = new List<List<BaseScheduler>>();
    private long _timeSlice = 16 * TimeSpan.TicksPerMillisecond;

    public override void Initialize(int threadCount)
    {
        base.Initialize(threadCount);

        for (int i = 0; i < threadCount; ++i)
        {
            _threadAccessList.Add(new List<BaseScheduler>());
        }
    }

    public void Add(BaseScheduler scheduler)
    {
        _schedulerList.Add(scheduler);

        for (int i = 0; i < _threadAccessList.Count; ++i)
        {
            _threadAccessList[i].Add(scheduler);
        }
    }

    public void Add(byte threadIndex, BaseScheduler scheduler)
    {
        if (_threadAccessList.Count <= threadIndex)
        {
            throw new Exception($"Thread Index Error. Count:{_threadAccessList.Count} Index: {threadIndex}");
        }

        _schedulerList.Add(scheduler);
        _threadAccessList[threadIndex].Add(scheduler);
    }

    public void Add(byte min, byte max, BaseScheduler scheduler)
    {
        if (min >= max)
        {
            throw new Exception($"min max value Error. min: {min} max:{max}");
        }

        if (_threadAccessList.Count <= max)
        {
            throw new Exception($"Thread Index Error. Count:{_threadAccessList.Count} Index: {max}");
        }

        _schedulerList.Add(scheduler);
        for (byte i = min; i <= max; ++i)
        {
            _threadAccessList[i].Add(scheduler);
        }
    }

    public override void Process()
    {
        long start = DateTime.UtcNow.Ticks;

        var schedulerList = _threadAccessList[BaseThread.AccessIndex];

        int count = schedulerList.Count;
        for (int i = 0; i < count; ++i)
        {
            try
            {
                var current = DateTime.UtcNow.Ticks;
                var slice = (current > (start + _timeSlice)) ? 0 : (start + _timeSlice - current);
                schedulerList[i].Process(slice / TimeSpan.TicksPerMillisecond);
            }
            catch (Exception ex)
            {
                LogSystem.Log.Error(ex);
            }
        }
    }

    public T Get<T>() where T : BaseScheduler
    {
        for (int i = 0; i < _schedulerList.Count; i++)
        {
            if (typeof(T) == _schedulerList[i].GetType())
            {
                return (T)_schedulerList[i];
            }
        }

        return default(T);
    }
}
