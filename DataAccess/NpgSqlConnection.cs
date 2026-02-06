using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace NIK.CORE.DOMAIN.DataAccess;

/// <summary>
/// PostgreSQL implementation of <see cref="ISqlConnection"/>.
/// </summary>
/// <remarks>
/// This class manages PostgreSQL database connections using Npgsql.
/// <para>
/// It supports:
/// <list type="bullet">
///   <item>Reusing a single open connection within the instance lifetime</item>
///   <item>Creating independent connections when required</item>
/// </list>
/// </para>
/// <para>
/// Intended for low-level data access (e.g. Dapper, raw SQL),
/// separate from EF Core <see cref="DbContext"/> usage.
/// </para>
/// </remarks>
public class NpgSqlConnection : ISqlConnection, IDisposable
{
    private readonly string _connectionString;
    private IDbConnection? _connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgSqlConnection"/> class.
    /// </summary>
    /// <param name="connectionString">
    /// The PostgreSQL connection string.
    /// </param>
    public NpgSqlConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Gets a reusable open PostgreSQL connection.
    /// </summary>
    /// <remarks>
    /// If no connection exists or the existing connection is not open,
    /// a new <see cref="NpgsqlConnection"/> is created and opened.
    /// <para>
    /// The same connection instance is reused for subsequent calls
    /// until this object is disposed.
    /// </para>
    /// </remarks>
    /// <returns>
    /// An open <see cref="IDbConnection"/> instance.
    /// </returns>
    public IDbConnection GetOpenConnection()
    {
        if (_connection is not { State: ConnectionState.Open })
        {
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();
        }

        return _connection;
    }

    /// <summary>
    /// Creates and opens a new PostgreSQL connection.
    /// </summary>
    /// <remarks>
    /// This method always returns a new connection instance.
    /// The caller is responsible for managing the connection lifecycle
    /// and disposing it after use.
    /// </remarks>
    /// <returns>
    /// A newly opened <see cref="IDbConnection"/> instance.
    /// </returns>
    public IDbConnection CreateNewConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Gets the PostgreSQL connection string.
    /// </summary>
    /// <returns>
    /// The configured connection string.
    /// </returns>
    public string GetConnectionString()
    {
        return _connectionString;
    }

    /// <summary>
    /// Releases the managed database connection resources.
    /// </summary>
    /// <remarks>
    /// Disposes the internally cached connection if it is open.
    /// </remarks>
    public void Dispose()
    {
        if (_connection is { State: ConnectionState.Open })
        {
            _connection.Dispose();
        }
    }
}
