using System.Threading;

namespace BlindServerCore.Threads;

public class SleepScheduler : BaseScheduler
{
    public override void Process(long slice)
    {
        if (slice > 0)
        {
            Thread.Sleep((int)slice);
        }
    }
}
