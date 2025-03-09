namespace BlindServerCore.Threads;

public class WorkThread : BaseThread
{
    protected ThreadContainer _container;

    public WorkThread(ThreadContainer container, byte pos)
        : base(pos)
    {
        _container = container;
    }

    public override void Process()
    {
        _container.Process();
    }
}
