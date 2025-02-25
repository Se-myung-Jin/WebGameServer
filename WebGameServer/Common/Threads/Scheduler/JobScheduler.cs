namespace Common;

public class JobScheduler : PerformanceCheckScheduler
{
    private ConcurrentQueue<Job> _queue = new ConcurrentQueue<Job>();

    protected override void OnProcess(long slice)
    {
        int limitJobCount = 1000;

        while(_queue.TryDequeue(out var job) && limitJobCount > 0)
        {
            --limitJobCount;
            _elapseTimer.Start();

            try
            {
                job.Execute();
            }
            catch (Exception ex)
            {

            }

            _elapseTimer.Stop();

            if (_elapseTimer.ElapsedMilliseconds >= 300)
            {

            }

            _elapseTimer.Reset();
        }
    }

    public void Add(Job job)
    {
        _queue.Enqueue(job);
    }
}
