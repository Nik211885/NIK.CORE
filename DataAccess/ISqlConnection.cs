using System.Data;

namespace NIK.CORE.DOMAIN.DataAccess;

/// <summary>
/// Defines an abstraction for creating and managing SQL database connections.
/// </summary>
/// <remarks>
/// This interface provides a unified way to obtain database connections
/// while hiding provider-specific implementation details.
/// It is typically used for low-level data access scenarios such as
/// raw SQL execution or Dapper-based queries.
/// </remarks>
public interface ISqlConnection
{
    /// <summary>
    /// Gets an open database connection.
    /// </summary>
    /// <remarks>
    /// Implementations should ensure that the returned connection
    /// is already opened and ready for use.
    /// </remarks>
    /// <returns>
    /// An open <see cref="IDbConnection"/> instance.
    /// </returns>
    IDbConnection GetOpenConnection();

    /// <summary>
    /// Creates a new database connection instance.
    /// </summary>
    /// <remarks>
    /// The returned connection is not guaranteed to be open.
    /// Callers are responsible for opening and disposing the connection.
    /// </remarks>
    /// <returns>
    /// A new <see cref="IDbConnection"/> instance.
    /// </returns>
    IDbConnection CreateNewConnection();

    /// <summary>
    /// Gets the database connection string.
    /// </summary>
    /// <remarks>
    /// This method is primarily intended for diagnostics, logging,
    /// or provider-specific initialization logic.
    /// </remarks>
    /// <returns>
    /// The connection string used to connect to the database.
    /// </returns>
    string GetConnectionString();
}
