using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.Contracts.Caching;
using NIK.CORE.DOMAIN.Enums;

namespace NIK.CORE.DOMAIN.Implements.Caching;

/// <summary>
///     Factory used to resolve <see cref="ICache"/> implementations
///     based on the specified <see cref="CachingProvider"/>.
///     <para>
///     This factory relies on keyed dependency injection to support
///     multiple caching providers (e.g. InMemory, Redis) at runtime.
///     </para>
/// </summary>
public class CacheFactory : ICacheFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of <see cref="CacheFactory"/>.
    /// </summary>
    /// <param name="serviceProvider">
    ///     The service provider used to resolve keyed cache services.
    /// </param>
    public CacheFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///     Creates an <see cref="ICache"/> instance based on the given
    ///     caching provider.
    /// </summary>
    /// <param name="provider">
    ///     The caching provider to resolve (e.g. InMemory, Redis).
    /// </param>
    /// <returns>
    ///     An <see cref="ICache"/> implementation associated with
    ///     the specified provider.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the requested caching provider has not been registered
    ///     in the dependency injection container.
    /// </exception>
    public ICache Create(CachingProvider provider)
    {
        var cachingService =
            _serviceProvider.GetKeyedService<ICache>(provider.ToString());
        ArgumentNullException.ThrowIfNull(cachingService);
        return cachingService;
    }
}