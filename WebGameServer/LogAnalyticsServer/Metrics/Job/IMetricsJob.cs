namespace LogAnalyticsServer.Metrics.Job;

public interface IMetricsJob
{
    TimeSpan Interval { get; }
    TimeSpan? DailyTime { get; }
    Task ExecuteAsync(CancellationToken token);
}