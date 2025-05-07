using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;

namespace BlindServerCore.Database;

public class SqliteContext : IDisposable
{
    private readonly string _connectionString;
    private readonly SqliteConnection? _memoryConnection;
    private bool _disposed;

    static SqliteContext()
    {
        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
    }

    public SqliteContext(string path)
    {
        _connectionString = $"Data Source={path}";
    }

    public SqliteContext(string databaseName, bool sharedMemory)
    {
        _connectionString = $"Data Source={databaseName};Mode=Memory;Cache=Shared";
        _memoryConnection = new SqliteConnection(_connectionString);
        _memoryConnection.Open();
    }

    public List<T> Query<T>(string query)
    {
        using var conn = GetConnectionIfNeeded();
        return conn.Query<T>(query).ToList();
    }

    public async Task<List<T>> QueryAsync<T>(string query)
    {
        using var conn = GetConnectionIfNeeded();
        return (await conn.QueryAsync<T>(query)).ToList();
    }

    public T? QueryFirst<T>(string query)
    {
        using var conn = GetConnectionIfNeeded();
        return conn.QueryFirstOrDefault<T>(query);
    }

    public async Task<T?> QueryFirstAsync<T>(string query)
    {
        using var conn = GetConnectionIfNeeded();
        return await conn.QueryFirstOrDefaultAsync<T>(query);
    }

    private SqliteConnection GetConnectionIfNeeded()
    {
        if (_memoryConnection != null)
            return _memoryConnection;

        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        return conn;
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;

        _memoryConnection?.Dispose();
    }
}
