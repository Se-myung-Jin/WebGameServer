using System.Diagnostics;

namespace Common;

public class PerformanceCheckScheduler : BaseScheduler
{
    protected Stopwatch _elapseTimer = new Stopwatch();

    public override void Process(long slice)
    {
        if (Interlocked.CompareExchange(ref _threadId, Thread.CurrentThread.ManagedThreadId, 0) != 0)
        {
            return;
        }

        try
        {
            OnProcess(slice);
        }
        catch (Exception ex)
        {

        }

        _elapseTimer.Reset();

        Interlocked.Exchange(ref _threadId, 0);
    }
}
