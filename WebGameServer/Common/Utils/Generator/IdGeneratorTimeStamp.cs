using System.Diagnostics;

namespace Common;

public abstract class IdGeneratorTimeStamp
{
    protected long _startTimeMs;
    protected long _lastTime;

    protected Stopwatch _stopWatch = new Stopwatch();

    public IdGeneratorTimeStamp()
    {
        _startTimeMs = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        _stopWatch.Start();
    }

    protected virtual long TimeGen()
    {
        long time = GetTime();
        if (time < _lastTime)
        {
            time = AdjustGetTime();
        }

        return time;
    }

    private long GetTime()
    {
        return DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
    }

    private long AdjustGetTime()
    {
        return _startTimeMs + _stopWatch.ElapsedMilliseconds;
    }
}
