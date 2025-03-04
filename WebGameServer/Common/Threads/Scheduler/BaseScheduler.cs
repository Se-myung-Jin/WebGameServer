namespace Common;

public abstract class BaseScheduler
{
    protected int _threadId = 0;
    protected bool _enableMultiThread = false;

    public virtual void Initialize()
    {

    }

    public virtual void Process(long slice)
    {
        if (_enableMultiThread == false)
        {
            if (Interlocked.CompareExchange(ref _threadId, Thread.CurrentThread.ManagedThreadId, 0) != 0)
            {
                return;
            }
        }

        try
        {
            OnProcess(slice);
        }
        catch (Exception ex)
        {

        }

        if (_enableMultiThread == false)
        {
            Interlocked.Exchange(ref _threadId, 0);
        }
    }

    public virtual void Release() { }
    protected virtual void OnProcess(long slice) { }
}
