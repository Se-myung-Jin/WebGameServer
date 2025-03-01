namespace Common;

public class JobScheduler : PerformanceCheckScheduler
{
    private ConcurrentQueue<BaseJob> _queue = new ConcurrentQueue<BaseJob>();

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

            // TODO : 추후 오브젝트 풀 빼도 될 듯
            //JobFactory.Instance.Release(job as InstanceJob);

            _elapseTimer.Reset();
        }
    }

    public void Add(BaseJob job)
    {
        _queue.Enqueue(job);
    }
}
