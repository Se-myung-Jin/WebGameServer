using BlindServerCore.Log;
using System;

namespace BlindServerCore.Threads;

public interface IJob
{
    void Start();
    void SetTime();
    bool IsTimeOver(long timeTick);
    void Execute();
}

public abstract class BaseJob : IJob
{
    protected Action _action = null;
    public string Description { get; protected set; }

    public BaseJob() { }

    public BaseJob(Action action, string description)
    {
        _action = action;
        Description = description;
    }

    public virtual void Start() { }
    public virtual void SetTime() { }
    public virtual bool IsTimeOver(long timeTick) => true;
    public virtual void Execute()
    {
        try
        {
            _action?.Invoke();
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);
        }
    }

    public virtual bool ExecutePostProcess()
    {
        return true;
    }
}
