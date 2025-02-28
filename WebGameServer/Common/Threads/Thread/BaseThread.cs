namespace Common;

public class BaseThread
{
    [ThreadStatic]
    public static byte AccessIndex;
    public int ManagedThreadId => _thread.ManagedThreadId;
    protected Thread _thread = null;
    protected byte _accessIndex = 0;
    protected bool _run = false;
    protected bool _initialized = false;

    public BaseThread(byte index)
    {
        _accessIndex = index;
        _thread = new Thread(ThreadFunc);
        _thread.IsBackground = true;
    }

    public virtual void Start()
    {
        if (_run)
        {
            return;
        }

        _run = true;
        _thread.Start();
    }

    public virtual void Stop()
    {
        if (_run == false)
        {
            return;
        }

        _run = false;
        try
        {
            _thread.Join(30000);
        }
        catch (Exception e)
        {

        }
    }

    protected virtual void ThreadFunc(object arg)
    {
        if (_initialized == false)
        {
            AccessIndex = _accessIndex;
            _initialized = true;
        }

        while (_run)
        {
            try
            {
                Process();
            }
            catch (Exception e)
            {

            }
        }
    }

    public virtual void Process()
    {

    }
}