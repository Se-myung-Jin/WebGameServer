namespace LogAnalyticsServer.Metrics.Job;

public class DailyMetricsJob : IMetricsJob
{
    public TimeSpan Interval => TimeSpan.Zero;
    public TimeSpan? DailyTime => TimeSpan.FromHours(0);
    public async Task ExecuteAsync(CancellationToken token)
    {
        LogSystem.Log.Info($"Daily Metrics Job Execute ===============");

        var date = DateTime.UtcNow.Date;

        await MetricsService.ExecuteDailyMetricsAsync(date);
    }
}

public class HourlyMetricsJob : IMetricsJob
{
    public TimeSpan Interval => TimeSpan.FromHours(1);
    public TimeSpan? DailyTime => null;

    public async Task ExecuteAsync(CancellationToken token)
    {
        LogSystem.Log.Info($"Hourly Metrics Job Execute ===============");
    }
}

public class MinutelyMetricsJob : IMetricsJob
{
    public TimeSpan Interval => TimeSpan.FromMinutes(1);
    public TimeSpan? DailyTime => null;

    public async Task ExecuteAsync(CancellationToken token)
    {
        LogSystem.Log.Info($"Minutely Metrics Job Execute ===============");

        var now = TimeUtils.GetTime();
        var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

        await MetricsService.ExecuteMinutelyMetricsAsync(date);
    }
}

public class EverySecondMetricsJob : IMetricsJob
{
    public TimeSpan Interval => TimeSpan.FromSeconds(5);
    public TimeSpan? DailyTime => null;

    public async Task ExecuteAsync(CancellationToken token)
    {
        LogSystem.Log.Info($"Every Second Metrics Job Execute ===============");
    }
}