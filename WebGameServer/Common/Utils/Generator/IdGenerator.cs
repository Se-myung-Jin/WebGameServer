using System.Diagnostics;

namespace Common;

/// <summary>
/// Snowflake 방식을 이용한 유니크한 ID 생성
/// 주의사항) serverId가 겹치면..안됩니다.
/// </summary>
public class IdGenerator : IdGeneratorTimeStamp
{
    //|------42BIT-----14 BIT-----8
    private const long TWEPOCH = 1288834974657L;
    // 16383
    private const byte BIT_SEQ = 14;
    private const byte BIT_SERVER = 8;
    private const short TIME_SHIFT = BIT_SEQ + BIT_SERVER;
    private readonly object _syncObject = new ();
    private byte _serverId;
    private long _seq;

    public IdGenerator(byte serverId)
    {
        _serverId = serverId;
    }

    public long Create()
    {
        lock (_syncObject)
        {
            do
            {
                long curTime = TimeGen();
                if (_lastTime > curTime)
                {
                    continue;
                }

                if (curTime == _lastTime)
                {
                    long checkValue = 1L << BIT_SEQ - 1;
                    if (_seq + 1 >= checkValue)
                    {
                        continue;
                    }
                    _seq++;
                }
                else
                {
                    _seq = 0;
                }

                _lastTime = curTime;
                break;
            } while (true);
        }

        //return ((m_lastTime - TWEPOCH) << TIME_SHIFT) | (m_serverId << BIT_SEQ) | m_seq;

        var value = _lastTime - TWEPOCH << TIME_SHIFT;
        value |= (long)(_serverId << BIT_SEQ);
        value |= _seq;

        return value;
    }

    public int CheckDpulicate(int second)
    {
        var time = second * 1000;

        Stopwatch timer = new Stopwatch();
        timer.Start();

        Dictionary<long, long> checkMap = new Dictionary<long, long>();

        do
        {
            var id = Create();
            checkMap.Add(id, id);
        } while (timer.ElapsedMilliseconds < time);

        timer.Stop();

        return checkMap.Count;
    }
}