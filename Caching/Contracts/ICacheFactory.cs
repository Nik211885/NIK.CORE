using NIK.CORE.DOMAIN.Caching.Enums;

namespace NIK.CORE.DOMAIN.Caching.Contracts;
/// <summary>
///     Defines a factory for creating <see cref="ICache"/> instances
///     based on a specified caching provider.
///     <para>
///     This abstraction allows the application to dynamically select
///     a caching implementation (e.g. InMemory, Redis) at runtime
///     without coupling to concrete cache implementations.
///     </para>
/// </summary>
public interface ICacheFactory
{
    /// <summary>
    ///     Creates an <see cref="ICache"/> instance corresponding
    ///     to the specified <see cref="CachingProvider"/>.
    /// </summary>
    /// <param name="provider">
    ///     The caching provider type used to determine which
    ///     cache implementation should be returned.
    /// </param>
    /// <returns>
    ///     An <see cref="ICache"/> implementation associated with
    ///     the given caching provider.
    /// </returns>
    ICache Create(CachingProvider provider);
}
