namespace BlindServerCore.Utils;

public class TickTimer
{
    protected uint _intervalMillisecond;
    protected long _expiredTimeTick = -1;

    public TickTimer()
    {
    }

    public TickTimer(uint intervalMillisecond)
    {
        _intervalMillisecond = intervalMillisecond;
    }

    public void Start(uint delay = 0)
    {
        _expiredTimeTick = TimeUtils.GetTimeTickAddMilliSecond(_intervalMillisecond + delay);
    }

    public void Stop()
    {
        _expiredTimeTick = -1;
    }

    public void Restart(uint delay = 0)
    {
        Start(delay);
    }

    public int GetRemainMilliSecond()
    {
        if (_expiredTimeTick == 0)
        {
            return -1;
        }

        return (int)_expiredTimeTick.GetRemainMilliSecond();
    }

    public int GetRemainSecond()
    {
        if (_expiredTimeTick == 0)
        {
            return -1;
        }

        return (int)_expiredTimeTick.GetRemainSecond();
    }
    public long GetExpireTimeTick() => _expiredTimeTick;

    public bool IsExpire() => IsExpire(TimeUtils.GetTimeTick());
    public bool IsExpire(long tick) => _expiredTimeTick > 0 && tick >= _expiredTimeTick;
    public void SetInterval(uint intervalMillisecond) => _intervalMillisecond = intervalMillisecond;
    public uint GetInterval() => _intervalMillisecond;
    public bool IsStop() => _expiredTimeTick == -1;
}