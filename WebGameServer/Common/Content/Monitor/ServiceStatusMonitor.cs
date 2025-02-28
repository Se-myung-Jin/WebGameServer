using System.Diagnostics;

namespace Common;

public class ServiceStatusMonitor
{
    protected ServerStatusDao _data;
    protected bool _enable = false;
    protected bool _initialize = false;

    private long _oldTotalProcessorTimeTick = 0;
    private long _oldTimeTick = 0;
    private float _highCpuUsage = 0.0f;
    private Process _process;

    public void Initialize(ServerStatusDao data)
    {
        Console.WriteLine($"ServiceStatusMonitor. VERSION : {data.CommitHash} PID: {Environment.ProcessId}");

        _data = data;
        _process = System.Diagnostics.Process.GetCurrentProcess();
    }

    public void SetEnable(bool enable)
    {
        _enable = enable;
    }

    public void SetIdAndPort(uint id, ushort port)
    {
        _data.ServerId = id;
        _data.Port = port;
    }

    public void Process()
    {
        UpdateData();

        if (_enable)
        {
            
        }
    }

    protected virtual void UpdateData()
    {
        _process.Refresh();
        _data.CpuPercent = GetCpuUsage();
        _data.HighCpuPercent = GetHighCpuUsage();
        _data.MemoryGb = GetRamUsage();
        _data.HighMemoryGb = GetHighRamUsage();

        Console.WriteLine($"CPU Percent: {_data.CpuPercent}, High CPU Percent: {_data.HighCpuPercent}, Memory: {_data.MemoryGb}, HighMemoryGb: {_data.HighMemoryGb}");
    }

    private float GetCpuUsage()
    {
        long currentTotalProcessorTimeTick = _process.TotalProcessorTime.Ticks;
        long currentTimeTick = DateTime.UtcNow.Ticks;

        long cpuTicks = currentTotalProcessorTimeTick - _oldTotalProcessorTimeTick;
        long timeElapsed = currentTimeTick - _oldTimeTick;

        //CASE: WINDOW 일 경우 주석 해제
        cpuTicks /= Environment.ProcessorCount;
        float cpuUsage = (float)cpuTicks / timeElapsed * 100.0f;

        _oldTotalProcessorTimeTick = currentTotalProcessorTimeTick;
        _oldTimeTick = currentTimeTick;

        if (_highCpuUsage < cpuUsage)
        {
            _highCpuUsage = cpuUsage;
        }

        return cpuUsage;
    }

    private float GetHighCpuUsage() => _highCpuUsage;

    private float GetRamUsage()
    {
        long ramUsageBytes = _process.WorkingSet64;
        float ramUsageGB = ramUsageBytes / (1024f * 1000f * 1000f);

        return ramUsageGB;
    }

    private float GetHighRamUsage() => _process.PeakWorkingSet64 / (1024f * 1000f * 1000f);
}