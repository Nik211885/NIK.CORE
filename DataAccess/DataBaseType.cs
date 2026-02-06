namespace NIK.CORE.DOMAIN.DataAccess;

/// <summary>
/// Represents the supported database providers used by the application.
/// </summary>
/// <remarks>
/// This enumeration is used to distinguish database-specific configurations,
/// behaviors, or infrastructure components (e.g., connection setup,
/// migrations, execution strategies).
/// </remarks>
public enum DataBaseType
{
    /// <summary>
    /// PostgreSQL database provider.
    /// </summary>
    Postgres,
}
