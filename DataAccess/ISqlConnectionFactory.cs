namespace NIK.CORE.DOMAIN.DataAccess;

/// <summary>
/// Factory interface for creating SQL connection providers
/// based on the specified database type.
/// </summary>
/// <remarks>
/// This abstraction allows the application to support multiple
/// database engines (e.g. PostgreSQL, SQL Server, etc.)
/// without coupling business logic to concrete connection implementations.
/// <para>
/// Typical usage:
/// <list type="bullet">
///   <item>Select a database type at runtime</item>
///   <item>Resolve the corresponding <see cref="ISqlConnection"/> implementation</item>
/// </list>
/// </para>
/// </remarks>
public interface ISqlConnectionFactory
{
    /// <summary>
    /// Creates an <see cref="ISqlConnection"/> instance for the specified database type.
    /// </summary>
    /// <param name="dataBaseType">
    /// The type of database for which the connection should be created.
    /// </param>
    /// <returns>
    /// An <see cref="ISqlConnection"/> implementation matching the given database type.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when the specified <paramref name="dataBaseType"/> is not supported.
    /// </exception>
    ISqlConnection Create(DataBaseType dataBaseType);
}
