using Dapper;
using System.Reflection;
using DBContext = BlindServerCore.Database.DatabaseContextContainer;

namespace LogAggregationServer;

public class LogTableGenerator
{
    public static readonly Dictionary<string, Type> LogTypeMapping = new Dictionary<string, Type>();

    public static async Task UpdateLogTableAsync()
    {
        var query = CreateLogTableQuery();

        if (!string.IsNullOrEmpty(query))
        {
            using var conn = DBContext.Instance.MySql.GetConnection(MySqlKind.Write);

            await conn.ExecuteAsync(query);
        }
    }

    private static string CreateLogTableQuery()
    {
        var stringBuilder = new StringBuilder();

        var list = CommonCustomAttribute.FindClassAttribute(typeof(LogTableAttribute));
        foreach (var (type, _) in list)
        {
            var attribute = type.GetCustomAttribute<LogTableAttribute>();
            if (attribute == null) continue;

            string tableName = GetLogTableName(attribute);
            string nextMonthTableName = GetNextMonthLogTableName(attribute);

            if (!LogTypeMapping.ContainsKey(tableName))
            {
                string createTableQuery = GenerateLogTableQuery(type, attribute, tableName);
                stringBuilder.Append(createTableQuery);
                LogTypeMapping[tableName] = type;
            }

            if (!string.IsNullOrEmpty(nextMonthTableName) && !LogTypeMapping.ContainsKey(nextMonthTableName))
            {
                string nextMonthTableQuery = GenerateLogTableQuery(type, attribute, nextMonthTableName);
                stringBuilder.Append(nextMonthTableQuery);
                LogTypeMapping[nextMonthTableName] = type;
            }
        }

        return stringBuilder.ToString();
    }

    private static string GetLogTableName(LogTableAttribute attribute)
    {
        return attribute.UseMonthlyPartition ? $"{attribute.TableName}_{TimeUtils.GetTime():yyyyMM}" : attribute.TableName;
    }

    private static string GetNextMonthLogTableName(LogTableAttribute attribute)
    {
        return attribute.UseMonthlyPartition ? $"{attribute.TableName}_{TimeUtils.GetTime().AddMonths(1):yyyyMM}" : string.Empty;
    }

    private static string GenerateLogTableQuery(Type type, LogTableAttribute attribute, string tableName)
    {
        var properties = type.GetProperties();
        var columns = new List<string>(20);

        columns.Add($"Seq BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY");

        foreach (var p in properties)
        {
            string sqlType = p.PropertyType switch
            {
                Type t when t == typeof(byte) => "TINYINT UNSIGNED",
                Type t when t == typeof(sbyte) => "TINYINT",
                Type t when t == typeof(ushort) => "SMALLINT UNSIGNED",
                Type t when t == typeof(short) => "SMALLINT",
                Type t when t == typeof(uint) => "INT UNSIGNED",
                Type t when t == typeof(int) => "INT",
                Type t when t == typeof(ulong) => "BIGINT UNSIGNED",
                Type t when t == typeof(long) => "BIGINT",
                Type t when t == typeof(float) => "FLOAT",
                Type t when t == typeof(double) => "DOUBLE",
                Type t when t == typeof(decimal) => "DECIMAL(18,6)",
                Type t when t == typeof(bool) => "TINYINT(1)",
                Type t when t == typeof(char) => "CHAR(1)",
                Type t when t == typeof(string) => "VARCHAR(255)",
                Type t when t == typeof(DateTime) => "DATETIME NOT NULL",
                _ => throw new NotSupportedException($"Unsupported type: {p.PropertyType}")
            };

            columns.Add($"{p.Name} {sqlType}");
        }

        var indexStatements = new List<string>(4);

        if (attribute.SingleIndexes?.Length > 0)
        {
            foreach (var index in attribute.SingleIndexes)
            {
                indexStatements.Add($"INDEX ({index})");
            }
        }

        if (attribute.CompositeIndexes?.Count > 0)
        {
            foreach (var compositeIndex in attribute.CompositeIndexes)
            {
                indexStatements.Add($"INDEX ({string.Join(", ", compositeIndex)})");
            }
        }

        if (attribute.UniqueIndexes?.Count > 0)
        {
            foreach (var uniqueIndex in attribute.UniqueIndexes)
            {
                indexStatements.Add($"UNIQUE INDEX ({string.Join(", ", uniqueIndex)})");
            }
        }

        return $@"
        CREATE TABLE IF NOT EXISTS {tableName} (
            {string.Join(", ", columns)}
            {(indexStatements.Count > 0 ? ", " + string.Join(", ", indexStatements) : "")}
        );";
    }
}
