using Microsoft.Extensions.Caching.Memory;
using NIK.CORE.DOMAIN.Caching.Contracts;

namespace NIK.CORE.DOMAIN.Caching.Implements;

/// <summary>
///     Provides an in-memory cache implementation using
///     <see cref="IMemoryCache"/>.
/// </summary>
/// <remarks>
///     This implementation stores cached data in application memory
///     and is suitable for single-instance or development scenarios.
/// </remarks>
public class MemoryCache : ICache
{
    private readonly IMemoryCache _memoryCache;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);
    /// <summary>
    ///     Initializes a new instance of the <see cref="MemoryCache"/> class.
    /// </summary>
    /// <param name="memoryCache">
    ///     The underlying memory cache implementation.
    /// </param>
    public MemoryCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    /// <summary>
    ///     Retrieves a cached value associated with the specified key
    ///     and deserializes it to the specified type.
    /// </summary>
    /// <typeparam name="TResponse">
    ///     The expected type of the cached value.
    /// </typeparam>
    /// <param name="key">
    ///     The unique key identifying the cached value.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation
    ///     and contains the cached value if found; otherwise, null.
    /// </returns>
    public ValueTask<TResponse?> Get<TResponse>(
        string key,
        CancellationToken cancellationToken = default)
    {
        TResponse? data = _memoryCache.Get<TResponse>(key);
        return ValueTask.FromResult(data);
    }

    /// <summary>
    ///     Retrieves a cached string value associated with the specified key.
    /// </summary>
    /// <param name="key">
    ///     The unique key identifying the cached value.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation
    ///     and contains the cached string value if found; otherwise, null.
    /// </returns>
    public ValueTask<string?> Get(
        string key,
        CancellationToken cancellationToken = default)
    {
        string? data = _memoryCache.Get<string>(key);
        return ValueTask.FromResult(data);
    }

    /// <summary>
    ///     Stores a value in the cache with the specified key.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value to cache.
    /// </typeparam>
    /// <param name="key">
    ///     The unique key identifying the cached value.
    /// </param>
    /// <param name="data">
    ///     The value to be stored in the cache.
    /// </param>
    /// <param name="expiration">
    ///     The optional expiration time for the cached value.
    ///     If not specified, a default expiration will be applied.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    public ValueTask Set<T>(
        string key,
        T data,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        expiration ??= DefaultExpiration;
        _memoryCache.Set(key, data, (TimeSpan)expiration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Stores an object value in the cache with the specified key.
    /// </summary>
    /// <param name="key">
    ///     The unique key identifying the cached value.
    /// </param>
    /// <param name="data">
    ///     The object to be stored in the cache.
    /// </param>
    /// <param name="expiration">
    ///     The optional expiration time for the cached value.
    ///     If not specified, a default expiration will be applied.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    public ValueTask Set(
        string key,
        object data,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        expiration ??= DefaultExpiration;
        _memoryCache.Set(key, data, (TimeSpan)expiration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Stores a string value in the cache with the specified key.
    /// </summary>
    /// <param name="key">
    ///     The unique key identifying the cached value.
    /// </param>
    /// <param name="data">
    ///     The string value to be stored in the cache.
    /// </param>
    /// <param name="expiration">
    ///     The optional expiration time for the cached value.
    ///     If not specified, a default expiration will be applied.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    public ValueTask Set(
        string key,
        string data,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        expiration ??= DefaultExpiration;
        _memoryCache.Set(key, data, (TimeSpan)expiration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Removes the cached value associated with the specified key.
    /// </summary>
    /// <param name="key">
    ///     The unique key identifying the cached value.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    public ValueTask Remove(
        string key,
        CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        return ValueTask.CompletedTask;
    }
}
