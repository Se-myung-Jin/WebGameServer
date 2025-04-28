using Dapper;
using DBContext = BlindServerCore.Database.DatabaseContextContainer;

namespace LogAnalyticsServer;

public class MetricsService
{
    public static async Task ExecuteDailyMetricsAsync(DateTime date)
    {
        using (var conn = DBContext.Instance.MySql.GetConnection(MySqlKind.Write))
        {
            conn.Open();

            var dauQuery = MetricsQueryGenerator.GetDAUQuery(date, conn);
            Console.WriteLine("[DAU Query]");
            Console.WriteLine(dauQuery);

            var wauQuery = MetricsQueryGenerator.GetWAUQuery(date, conn);
            Console.WriteLine("[WAU Query]");
            Console.WriteLine(wauQuery);

            var mauQuery = MetricsQueryGenerator.GetMAUQuery(date, conn);
            Console.WriteLine("[MAU Query]");
            Console.WriteLine(mauQuery);

            var nuQuery = MetricsQueryGenerator.GetNUQuery(date);
            Console.WriteLine("[NU Query]");
            Console.WriteLine(nuQuery);

            var retention_1 = MetricsQueryGenerator.GetRetentionRateQueryV2(1, date, conn);
            Console.WriteLine("[Retention 1Day Rate Query]");
            Console.WriteLine(retention_1);

            var retention_7 = MetricsQueryGenerator.GetRetentionRateQueryV2(7, date, conn);
            Console.WriteLine("[Retention 7Day Rate Query]");
            Console.WriteLine(retention_7);

            var retention_14 = MetricsQueryGenerator.GetRetentionRateQueryV2(14, date, conn);
            Console.WriteLine("[Retention 14Day Rate Query]");
            Console.WriteLine(retention_14);

            var retention_30 = MetricsQueryGenerator.GetRetentionRateQueryV2(30, date, conn);
            Console.WriteLine("[Retention 30Day Rate Query]");
            Console.WriteLine(retention_30);

            var result = (await conn.QueryAsync<int>(dauQuery)).ToList();
            Console.WriteLine($"[Result] : {result.FirstOrDefault()}");

            var result2 = (await conn.QueryAsync<int>(wauQuery)).ToList();
            Console.WriteLine($"[Result2] : {result2.FirstOrDefault()}");

            var result3 = (await conn.QueryAsync<int>(mauQuery)).ToList();
            Console.WriteLine($"[Result3] : {result3.FirstOrDefault()}");

            var result4 = (await conn.QueryAsync<int>(nuQuery)).ToList();
            Console.WriteLine($"[Result4] : {result4.FirstOrDefault()}");

            var result5 = (await conn.QueryAsync<decimal>(retention_1)).ToList();
            Console.WriteLine($"[Result5] : {result5.FirstOrDefault()}");

            var result6 = (await conn.QueryAsync<decimal>(retention_7)).ToList();
            Console.WriteLine($"[Result6] : {result6.FirstOrDefault()}");

            var result7 = (await conn.QueryAsync<decimal>(retention_14)).ToList();
            Console.WriteLine($"[Result7] : {result7.FirstOrDefault()}");

            var result8 = (await conn.QueryAsync<decimal>(retention_30)).ToList();
            Console.WriteLine($"[Result8] : {result8.FirstOrDefault()}");

            var logDailyKpi = new LogKPIMetricsDao()
            {
                Dau = (uint)result.FirstOrDefault(),
                Wau = (uint)result2.FirstOrDefault(),
                Mau = (uint)result3.FirstOrDefault(),
                NewUsers = (uint)result4.FirstOrDefault(),
                Retention_1 = (float)result5.FirstOrDefault(),
                Retention_7 = (float)result6.FirstOrDefault(),
                Retention_14 = (float)result7.FirstOrDefault(),
                Retention_30 = (float)result8.FirstOrDefault(),
                LogTime = TimeUtils.GetTime().Date
            };

            var query = $"Insert Into LogKPIMetrics (Dau, Wau, Mau, NewUsers, Retention_1, Retention_7, Retention_14, Retention_30, LogTime) VALUES (@Dau, @Wau, @Mau, @NewUsers, @Retention_1, @Retention_7, @Retention_14, @Retention_30, @LogTime)";

            await conn.ExecuteAsync(query, logDailyKpi);
        }
    }

    public static async Task ExecuteMinutelyMetricsAsync(DateTime date)
    {
        var CCUQuery = MetricsQueryGenerator.GetCCUQuery(date);

        using var conn = DBContext.Instance.MySql.GetConnection(MySqlKind.Write);
        conn.Open();

        var metrics = new LogCCUDao() { WorldId = 1, Users = 1, LogTime = date };
        await conn.ExecuteAsync(CCUQuery, metrics);
    }
}