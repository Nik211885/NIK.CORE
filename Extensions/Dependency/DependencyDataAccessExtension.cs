using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.DataAccess;

namespace NIK.CORE.DOMAIN.Extensions.Dependency;

/// <summary>
/// Provides dependency injection extensions for configuring SQL data access.
/// </summary>
/// <remarks>
/// This extension supports two distinct scenarios:
/// <list type="bullet">
///   <item>
///     <b>Single database</b>: registers <see cref="ISqlConnection"/> directly,
///     allowing consumers to inject it without additional abstraction.
///   </item>
///   <item>
///     <b>Multiple databases</b>: registers keyed <see cref="ISqlConnection"/> instances
///     and exposes <see cref="ISqlConnectionFactory"/> for runtime database selection.
///   </item>
/// </list>
/// <para>
/// This design keeps the common single-database case simple while still
/// supporting advanced multi-database scenarios when needed.
/// </para>
/// </remarks>
public static class DependencyDataAccessExtension
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers SQL connection services based on the provided configuration.
        /// </summary>
        /// <param name="configuration">
        /// A delegate used to configure one or more database connection definitions.
        /// </param>
        /// <returns>
        /// The same <see cref="IServiceCollection"/> instance for chaining.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no database configuration is provided.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// Thrown when a configured database type is not supported.
        /// </exception>
        public IServiceCollection AddDataAccessSqlConnection(
            Action<DataAccessConfigs> configuration)
        {
            var configs = new DataAccessConfigs();
            configuration(configs);

            if (configs.Configs == null || configs.Configs.Count == 0)
            {
                throw new InvalidOperationException(
                    "No database configuration was provided.");
            }
            if (configs.Configs.Count == 1)
            {
                RegisterSingleDatabase(services, configs.Configs[0]);
            }
            else
            {
                RegisterMultipleDatabases(services, configs.Configs);
                services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
            }

            return services;
        }
    }

    /// <summary>
    /// Registers a single SQL connection without using keyed services.
    /// </summary>
    /// <param name="services">The DI container.</param>
    /// <param name="config">The database configuration.</param>
    private static void RegisterSingleDatabase(
        IServiceCollection services,
        DataAccessConfig config)
    {
        services.AddScoped<ISqlConnection>(_ => CreateSqlConnection(config));
    }

    /// <summary>
    /// Registers multiple SQL connections as keyed services.
    /// </summary>
    /// <param name="services">The DI container.</param>
    /// <param name="configs">The database configurations.</param>
    private static void RegisterMultipleDatabases(
        IServiceCollection services,
        IEnumerable<DataAccessConfig> configs)
    {
        foreach (var config in configs)
        {
            services.AddKeyedScoped<ISqlConnection>(
                config.DataBaseType.ToString(),
                (_, _) => CreateSqlConnection(config));
        }
    }

    /// <summary>
    /// Creates an <see cref="ISqlConnection"/> instance based on database type.
    /// </summary>
    /// <param name="config">The database configuration.</param>
    /// <returns>
    /// A concrete implementation of <see cref="ISqlConnection"/>.
    /// </returns>
    /// <exception cref="NotImplementedException">
    /// Thrown when the database type is not supported.
    /// </exception>
    private static ISqlConnection CreateSqlConnection(DataAccessConfig config)
    {
        return config.DataBaseType switch
        {
            DataBaseType.Postgres => new NpgSqlConnection(config.ConnectionString),
            _ => throw new NotImplementedException(
                $"Not support sql connection for database {config.DataBaseType}")
        };
    }
}

/// <summary>
/// Represents a collection of database access configurations.
/// </summary>
/// <remarks>
/// This class is used as a configuration container when registering
/// SQL connections in the dependency injection container.
/// <para>
/// It supports both single-database and multi-database scenarios.
/// </para>
/// </remarks>
public class DataAccessConfigs
{
    /// <summary>
    /// Gets or sets the list of database configurations.
    /// </summary>
    /// <remarks>
    /// Each entry defines how to connect to a specific database provider.
    /// </remarks>
    public List<DataAccessConfig> Configs { get; set; }
}

/// <summary>
/// Represents the configuration required to connect to a specific database.
/// </summary>
/// <remarks>
/// This configuration is provider-agnostic and can be extended to support
/// additional database types (e.g., PostgreSQL, SQL Server, MySQL).
/// </remarks>
public class DataAccessConfig
{
    /// <summary>
    /// Gets or sets the database provider type.
    /// </summary>
    /// <remarks>
    /// Determines which <see cref="ISqlConnection"/> implementation
    /// will be created at runtime.
    /// </remarks>
    public DataBaseType DataBaseType { get; set; }

    /// <summary>
    /// Gets or sets the connection string used to connect to the database.
    /// </summary>
    /// <remarks>
    /// This value should follow the provider-specific connection string format.
    /// </remarks>
    public string ConnectionString { get; set; }
}
