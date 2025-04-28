using System.Data;
using Dapper;

namespace LogAnalyticsServer;

public static class MetricsQueryGenerator
{
    private const string AuthTablePrefix = "LogAuth_";
    private const string NewAccountTable = "LogNewAccount";
    private const string ConcurrentUserTable = "LogCCU_";

    public static string GetDAUQuery(DateTime date, IDbConnection connection)
    {
        var targetDay = date.Date.AddDays(-1);
        var startTime = targetDay;
        var endTime = targetDay.AddDays(1).AddTicks(-1);
        string tableName = GetActiveUserTableName(startTime);

        if (!TableExists(tableName, connection))
        {
            return "-- Table not found for DAU.";
        }

        return $@"
            SELECT COUNT(DISTINCT UserId) AS DAU
            FROM `{tableName}`
            WHERE LogTime >= '{startTime:yyyy-MM-dd HH:mm:ss}' AND LogTime <= '{endTime:yyyy-MM-dd HH:mm:ss}';
        ";
    }

    public static string GetWAUQuery(DateTime date, IDbConnection connection)
    {
        var endDate = date.Date;
        var startDate = endDate.AddDays(-7);

        var availableTables = GetPartitionedTablesInRange(startDate, endDate, GetActiveUserTableName)
            .Where(t => TableExists(t, connection))
            .OrderBy(t => t)
            .ToList();

        if (availableTables.Count == 0)
        {
            return "-- No available tables for WAU.";
        }

        var unionQuery = string.Join("\nUNION ALL\n", availableTables.Select(table =>
            $@"
            SELECT UserId
            FROM `{table}`
            WHERE LogTime >= '{startDate:yyyy-MM-dd HH:mm:ss}' AND LogTime < '{endDate:yyyy-MM-dd HH:mm:ss}'
            "
        ));

        return $@"
        SELECT COUNT(DISTINCT UserId)
        FROM (
            {unionQuery}
        ) AS WAU;";
    }

    public static string GetMAUQuery(DateTime date, IDbConnection connection)
    {
        var endDate = date.Date;
        var startDate = endDate.AddDays(-30);

        var availableTables = GetPartitionedTablesInRange(startDate, endDate, GetActiveUserTableName)
            .Where(t => TableExists(t, connection))
            .OrderBy(t => t)
            .ToList();

        if (availableTables.Count == 0)
        {
            return "-- No available tables for MAU.";
        }

        var unionQuery = string.Join("\nUNION ALL\n", availableTables.Select(table =>
            $@"
            SELECT UserId
            FROM `{table}`
            WHERE LogTime >= '{startDate:yyyy-MM-dd HH:mm:ss}' AND LogTime < '{endDate:yyyy-MM-dd HH:mm:ss}'
            "
        ));

        return $@"
        SELECT COUNT(DISTINCT UserId)
        FROM (
            {unionQuery}
        ) AS MAU;";
    }

    public static string GetNUQuery(DateTime date)
    {
        var targetDay = date.Date.AddDays(-1);
        var startTime = targetDay;
        var endTime = targetDay.AddDays(1).AddTicks(-1);
        string tableName = GetNewUserAccountTableName();

        return $@"
            SELECT COUNT(UserId) AS NU
            FROM `{tableName}`
            WHERE CreateTime >= '{startTime:yyyy-MM-dd HH:mm:ss}' AND CreateTime <= '{endTime:yyyy-MM-dd HH:mm:ss}';
        ";
    }

    public static string GetCCUQuery(DateTime date)
    {
        string tableName = GetCCUTableName(date);

        return $@"
            INSERT INTO `{tableName}` (WorldId, Users, LogTime) VALUES (@WorldId, @Users, @LogTime);
        ";
    }

    private static string GetActiveUserTableName(DateTime date)
    {
        return $"{AuthTablePrefix}{date:yyyyMM}";
    }

    private static string GetNewUserAccountTableName()
    {
        return $"{NewAccountTable}";
    }

    private static string GetCCUTableName(DateTime date)
    {
        return $"{ConcurrentUserTable}{date:yyyyMM}";
    }

    public static string GetRetentionRateQuery(int day, DateTime baseDateUtc, IDbConnection connection)
    {
        // 기준일 0시 (UTC 기준)
        var baseDate = baseDateUtc.Date;

        // 로그인 날짜는 기준일 - 1일
        var loginStart = baseDate.AddDays(-1);
        var loginEnd = baseDate;

        // 가입 날짜는 로그인 시작일 - N일
        var joinStart = loginStart.AddDays(-day);
        var joinEnd = joinStart.AddDays(1);

        string loginTable = $"{AuthTablePrefix}{loginStart:yyyyMM}";
        if (!TableExists(loginTable, connection))
            return $"-- Table {loginTable} not found for Retention {day}.";

        return $@"
        SELECT 
            ROUND(
                COUNT(DISTINCT b.UserId) / 
                (SELECT COUNT(*) FROM `{NewAccountTable}` a 
                 WHERE a.CreateTime >= '{joinStart:yyyy-MM-dd HH:mm:ss}' 
                   AND a.CreateTime < '{joinEnd:yyyy-MM-dd HH:mm:ss}') 
                * 100, 2) AS Retention{day}
        FROM `{NewAccountTable}` a
        JOIN `{loginTable}` b ON a.UserId = b.UserId
        WHERE a.CreateTime >= '{joinStart:yyyy-MM-dd HH:mm:ss}' AND a.CreateTime < '{joinEnd:yyyy-MM-dd HH:mm:ss}'
          AND b.LogTime >= '{loginStart:yyyy-MM-dd HH:mm:ss}' AND b.LogTime < '{loginEnd:yyyy-MM-dd HH:mm:ss}';
        ";
    }

    public static string GetRetentionRateQueryV2(int day, DateTime baseDate, IDbConnection connection)
    {
        var loginDate = baseDate.Date.AddDays(-1); // 접속일 = 기준일 하루 전
        var loginEnd = loginDate.AddDays(1);

        var joinDate = loginDate.AddDays(-day);
        var joinEnd = joinDate.AddDays(1);

        string loginTable = $"{AuthTablePrefix}{loginDate:yyyyMM}";
        if (!TableExists(loginTable, connection))
            return $"-- Table {loginTable} not found for Retention {day}.";

        return $@"
        SELECT 
            IFNULL(COUNT(DISTINCT b.UserId) / NULLIF(COUNT(DISTINCT a.UserId), 0), 0) AS Retention{day}
        FROM `{NewAccountTable}` a
        LEFT JOIN `{loginTable}` b 
            ON a.UserId = b.UserId 
            AND b.LogTime >= '{loginDate:yyyy-MM-dd HH:mm:ss}' AND b.LogTime < '{loginEnd:yyyy-MM-dd HH:mm:ss}'
        WHERE a.CreateTime >= '{joinDate:yyyy-MM-dd HH:mm:ss}' AND a.CreateTime < '{joinEnd:yyyy-MM-dd HH:mm:ss}';
        ";
    }

    private static List<string> GetPartitionedTablesInRange(DateTime start, DateTime end, Func<DateTime, string> getterTable)
    {
        var tables = new HashSet<string>();
        var date = new DateTime(start.Year, start.Month, 1);
        var endMonth = new DateTime(end.Year, end.Month, 1);

        while (date <= endMonth)
        {
            tables.Add(getterTable.Invoke(date));
            date = date.AddMonths(1);
        }

        return tables.OrderBy(t => t).ToList();
    }

    private static bool TableExists(string tableName, IDbConnection connection)
    {
        const string query = @"
            SELECT COUNT(*) 
            FROM information_schema.tables 
            WHERE table_schema = 'log' AND table_name = @TableName;
        ";

        return connection.ExecuteScalar<int>(query, new { TableName = tableName }) > 0;
    }
}
