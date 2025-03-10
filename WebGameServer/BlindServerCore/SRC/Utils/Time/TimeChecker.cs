using BlindServerCore.Log;
using System;
using System.Diagnostics;

namespace BlindServerCore.Utils
{
    public struct ElapseTimer : IDisposable
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private string _description;
        private uint _warnningMillisecond;

        public ElapseTimer(uint warningContionMilliSecond, bool autoStart = true, string description = "")
        {
            _warnningMillisecond = warningContionMilliSecond;
            _description = description;
            if (autoStart)
            {
                Start();
            }
        }

        public void Start()
        {
            _stopwatch.Restart();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public long GetTick() => _stopwatch.ElapsedTicks;

        public long GetElapseMilliSecond() => _stopwatch.ElapsedMilliseconds;

        public void Dispose() 
        {
            Stop();

            if (_stopwatch.ElapsedMilliseconds > _warnningMillisecond) 
            {
                LogSystem.Log.Warn($"ElapseTimer {_description} [{_stopwatch.ElapsedMilliseconds}]ms");
            }
        }
    }
}
