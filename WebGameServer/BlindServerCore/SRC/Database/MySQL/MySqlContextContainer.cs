using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BlindServerCore.Database;

[AttributeUsage(AttributeTargets.Class)]
public class LogTableAttribute : Attribute
{
    public string TableName { get; }
    public bool UseMonthlyPartition { get; }
    public string[] SingleIndexes { get; }
    public List<List<string>> CompositeIndexes { get; }
    public List<List<string>> UniqueIndexes { get; }

    public LogTableAttribute(string tableName,
        bool useMonthlyPartition = false,
        string[] singleIndexes = null,
        string[] compositeIndexes = null,
        string[] uniqueIndexes = null)
    {
        TableName = tableName;
        UseMonthlyPartition = useMonthlyPartition;

        SingleIndexes = singleIndexes ?? Array.Empty<string>();
        CompositeIndexes = compositeIndexes != null ? ConvertToList(compositeIndexes) : new List<List<string>>();
        UniqueIndexes = uniqueIndexes != null ? ConvertToList(uniqueIndexes) : new List<List<string>>();
    }

    private static List<List<string>> ConvertToList(string[] indexes)
    {
        var list = new List<List<string>>();
        foreach (var index in indexes)
        {
            var splitIndex = index.Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (splitIndex.Count > 0)
            {
                list.Add(splitIndex);
            }
        }

        return list;
    }
}

public class MySqlContextContainer
{
    private readonly Dictionary<MySqlKind, MySqlConnection> _connectionMap = new();

    public void Initialize()
    {

    }

    public MySqlConnection Add(DBConfig config)
    {
        Enum.TryParse(config.Name, out MySqlKind typeName);
        
        var connection = new MySqlConnection(config.ConnectionString);
        _connectionMap[typeName] = connection;

        return connection;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    public MySqlConnection GetConnection(MySqlKind kind)
    {
        var conn = _connectionMap[kind];

        return new MySqlConnection(conn.ConnectionString);
    }

    public void Destroy()
    {
        foreach (var connection in _connectionMap.Values)
        {
            connection.Dispose();
        }

        _connectionMap.Clear();
    }
}