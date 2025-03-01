namespace Common;

public class InstanceJob : BaseJob
{
    public static JobScheduler OwnerProcess = null;

    public InstanceJob()
    { }

    public InstanceJob(Action action, string description)
        : base(action, description)
    {

    }

    public void Set(Action action, string description)
    {
        _action = action;
        Description = description;
    }

    public override void Start()
    {
        SetTime();

        OwnerProcess.Add(this);
    }

    public void Clear()
    {
        _action = null;
        Description = "";
    }
}