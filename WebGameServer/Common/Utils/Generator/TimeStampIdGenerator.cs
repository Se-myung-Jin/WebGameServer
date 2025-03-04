using System.Diagnostics;

namespace Common;

public class TimeStampIdGenerator
{
    protected static Stopwatch _globalStopwatch = new Stopwatch();
    protected static uint _globalStartEpochTime;
    protected uint _lastTimeSecond;
    protected uint _index;

    //4294967295
    //4194303000
    //22bit - 4194303 (3FFFFF), 10bit - 1023 (3FF)
    //23bit - 8388607 (7FFFFF) 9bit 511 (1FF)
    //private const uint CONST_TIME_STAMP = 0x3FFFFF;
    protected const uint CONST_MAX_SEQUENCE = 0x1FF;
    protected const byte SEQUENCE_BIT = 9;

    private readonly object _syncObject = new();

    public TimeStampIdGenerator(uint epochTime)
    {
        lock (_globalStopwatch)
        {
            if (_globalStopwatch.IsRunning == false)
            {
                _globalStartEpochTime = epochTime;
                _globalStopwatch.Start();
            }            
        }
    }

    public uint Create()
    {
        lock (_syncObject)
        {
            while (true)
            {
                var epochTime = GetEpoch();
                if (_lastTimeSecond != epochTime)
                {
                    _lastTimeSecond = epochTime;
                    _index = 1;
                }

                if (_index + 1 >= CONST_MAX_SEQUENCE)
                {
                    continue;
                }

                ++_index;
                return (epochTime << SEQUENCE_BIT) | _index;
            }
        }
    }

    protected uint GetEpoch() => (uint)(_globalStartEpochTime + (_globalStopwatch.ElapsedMilliseconds * 0.001f));
};
