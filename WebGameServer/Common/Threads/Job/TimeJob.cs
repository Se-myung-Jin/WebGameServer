namespace Common;

public class TimeJob : BaseJob
{
    public static TimeScheduler OwnerScheduler = null;

    protected long _elapseMilliSecond;
    protected long _interval;
    protected bool _repeat;

    public TimeJob(Action action, int intervalMillisecond, string description, bool repeat = true)
            : base(action, description)
    {
        if (intervalMillisecond <= 0)
        {
            throw new Exception("TimeScheduler Zero Not Support");
        }

        _repeat = repeat;
        _interval = intervalMillisecond * TimeSpan.TicksPerMillisecond;
    }

    public override void Start()
    {
        SetTime();

        OwnerScheduler.Add(this);
    }

    public override void SetTime()
    {
        _elapseMilliSecond = DateTime.UtcNow.Ticks + _interval;
    }

    public override bool ExecutePostProcess()
    {
        if (_repeat)
        {
            SetTime();
            
            return true;
        }

        return false;
    }

    public override bool IsTimeOver(long timeTick) => _elapseMilliSecond <= timeTick;
}
