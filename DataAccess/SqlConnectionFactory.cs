using Microsoft.Extensions.DependencyInjection;

namespace NIK.CORE.DOMAIN.DataAccess;

/// <summary>
/// Default implementation of <see cref="ISqlConnectionFactory"/> that resolves
/// SQL connection providers using dependency injection.
/// </summary>
/// <remarks>
/// This factory relies on <b>keyed services</b> to select the correct
/// <see cref="ISqlConnection"/> implementation based on <see cref="DataBaseType"/>.
/// <para>
/// Each database provider must be registered in the DI container
/// using a key that matches <see cref="DataBaseType"/>.
/// </para>
/// <para>
/// Example registration:
/// <code>
/// services.AddKeyedScoped&lt;ISqlConnection&gt;(
///     DataBaseType.Postgres.ToString(),
///     _ =&gt; new NpgSqlConnection(connectionString));
/// </code>
/// </para>
/// <para>
/// This design enables:
/// <list type="bullet">
///   <item>Multiple database engines in a single application</item>
///   <item>Clean separation between infrastructure and domain logic</item>
///   <item>Easy extension without modifying existing code</item>
/// </list>
/// </para>
/// </remarks>
public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlConnectionFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">
    /// The service provider used to resolve keyed SQL connection services.
    /// </param>
    public SqlConnectionFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates an <see cref="ISqlConnection"/> instance for the specified database type.
    /// </summary>
    /// <param name="dataBaseType">
    /// The database type used as the resolution key.
    /// </param>
    /// <returns>
    /// A resolved <see cref="ISqlConnection"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when no <see cref="ISqlConnection"/> is registered for the given database type.
    /// </exception>
    public ISqlConnection Create(DataBaseType dataBaseType)
    {
        var services = _serviceProvider.GetKeyedService<ISqlConnection>(
            dataBaseType.ToString());

        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
