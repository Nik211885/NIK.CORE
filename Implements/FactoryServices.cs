using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.Contracts;

namespace NIK.CORE.DOMAIN.Implements;

/// <summary>
///     Default implementation of <see cref="T"/>.
///     <para>
///     This factory resolves services from the dependency injection container
///     using a string-based key.
///     </para>
///     <para>
///     It is intended for scenarios where multiple implementations of the same
///     service contract are registered and selected at runtime.
///     </para>
/// </summary>
public class FactoryServices : IFactoryServices
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FactoryServices"/> class.
    /// </summary>
    /// <param name="serviceProvider">
    ///     The service provider used to resolve keyed services.
    /// </param>
    public FactoryServices(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///     Creates a service instance associated with the specified key.
    /// </summary>
    /// <param name="name">
    ///     The name key used to resolve the service implementation.
    /// </param>
    /// <returns>
    ///     An instance of <typeparamref name="T"/> registered with the given key.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when no service is registered with the specified key.
    /// </exception>
    public T Create<T>(string name)
    {
        var service = _serviceProvider.GetKeyedService<T>(name);
        ArgumentNullException.ThrowIfNull(service);
        return service;
    }
}
