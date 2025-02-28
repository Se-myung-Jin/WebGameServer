namespace Common;

public abstract class ThreadContainer
{
    protected List<WorkThread> _threadList = new List<WorkThread>();
    protected bool _run = false;

    public virtual void Initialize(int threadCount)
    {
        for (int i = 0; i < threadCount; i++)
        {
            _threadList.Add(new WorkThread(this, (byte)i));
        }
    }

    public virtual void Start()
    {
        if (_run)
        {
            return;
        }

        _run = true;
        for (int i = 0; i < _threadList.Count; i++)
        {
            _threadList[i].Start();
        }
    }

    public virtual void Stop()
    {
        if (_run == false)
        {
            return;
        }

        _run = false;
        for (int i = 0; i < _threadList.Count; ++i)
        {
            _threadList[i].Stop();
        }
    }

    public int GetThreadCount() => _threadList.Count;
    protected int GetThreadId(int index) => _threadList[index].ManagedThreadId;

    public abstract void Process();
}
