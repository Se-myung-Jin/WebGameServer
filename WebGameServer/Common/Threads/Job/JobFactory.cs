namespace Common;

public class JobFactory
{
    private static Lazy<JobFactory> _instance = new Lazy<JobFactory>(() => new JobFactory());
    public static JobFactory Instance => _instance.Value;
    ConcurrencyObjectPool<InstanceJob> _instanceJobPool = new ConcurrencyObjectPool<InstanceJob>();

    private JobFactory()
    {
        _instanceJobPool.Initialize(10000);
    }

    public InstanceJob CreateInstanceJob() => _instanceJobPool.Acuire();

    public void Release(InstanceJob data)
    {
        data.Clear();
        _instanceJobPool.Release(data);
    }
}