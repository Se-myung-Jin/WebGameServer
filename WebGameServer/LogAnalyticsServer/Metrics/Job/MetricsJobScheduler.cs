using LogAnalyticsServer.Metrics.Job;
using Microsoft.Extensions.Hosting;

public class MetricsJobScheduler : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly List<IMetricsJob> _jobs = new();

    private readonly List<Timer> _timers = new();

    public MetricsJobScheduler(IServiceProvider provider)
    {
        _provider = provider;

        _jobs.Add(new DailyMetricsJob());
        _jobs.Add(new HourlyMetricsJob());
        _jobs.Add(new MinutelyMetricsJob());
        _jobs.Add(new EverySecondMetricsJob());
    }

    protected override Task ExecuteAsync(CancellationToken token)
    {
        foreach (var job in _jobs)
        {
            if (job.DailyTime.HasValue)
            {
                ScheduleDailyJob(job, job.DailyTime.Value, token);
            }
            else if (job.Interval > TimeSpan.Zero)
            {
                ScheduleIntervalJob(job, job.Interval, token);
            }
        }

        return Task.CompletedTask;
    }

    private void ScheduleDailyJob(IMetricsJob job, TimeSpan runTimeUtc, CancellationToken token)
    {
        var now = DateTime.UtcNow;
        var nextRun = now.Date.Add(runTimeUtc);

        if (nextRun <= now)
        {
            nextRun = nextRun.AddDays(1);
        }

        var initialDelay = nextRun - now;

        var timer = new Timer(async _ =>
        {
            await job.ExecuteAsync(token);

            ScheduleDailyJob(job, runTimeUtc, token);

        }, null, initialDelay, Timeout.InfiniteTimeSpan);

        _timers.Add(timer);
    }

    private void ScheduleIntervalJob(IMetricsJob job, TimeSpan interval, CancellationToken token)
    {
        var timer = new Timer(async _ =>
        {
            await job.ExecuteAsync(token);
        }, null, TimeSpan.Zero, interval);

        _timers.Add(timer);
    }

    public override void Dispose()
    {
        foreach (var timer in _timers)
        {
            timer.Dispose();
        }

        base.Dispose();
    }
}
