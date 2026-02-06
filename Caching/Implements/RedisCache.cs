using System.Text.Json;
using NIK.CORE.DOMAIN.Caching.Contracts;
using StackExchange.Redis;

namespace NIK.CORE.DOMAIN.Caching.Implements;

/// <summary>
///     Redis-based implementation of <see cref="ICache"/>.
///     <para>
///     This cache uses <c>StackExchange.Redis</c> to store and retrieve
///     cached values from a Redis server.
///     </para>
///     <para>
///     All non-string values are serialized to JSON using
///     <see cref="System.Text.Json.JsonSerializer"/>.
///     </para>
/// </summary>
public class RedisCache : ICache
{
    /// <summary>
    ///     Default expiration time applied when no expiration is provided.
    ///     <para>
    ///     The default value is 5 minutes.
    ///     </para>
    /// </summary>
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    /// <summary>
    ///     Redis database instance used for cache operations.
    /// </summary>
    private readonly IDatabase _database;

    /// <summary>
    ///     Initializes a new instance of <see cref="RedisCache"/>.
    /// </summary>
    /// <param name="connectionMultiplexer">
    ///     The Redis connection multiplexer used to obtain
    ///     a database instance.
    /// </param>
    public RedisCache(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    /// <summary>
    ///     Retrieves a cached value by key and deserializes it
    ///     to the specified type.
    /// </summary>
    /// <typeparam name="TResponse">
    ///     The expected type of the cached value.
    /// </typeparam>
    /// <param name="key">
    ///     The cache key.
    /// </param>
    /// <param name="cancellationToken">
    ///     Cancellation token (not used by Redis operations).
    /// </param>
    /// <returns>
    ///     The cached value if found; otherwise <c>null</c>.
    /// </returns>
    public async ValueTask<TResponse?> Get<TResponse>(
        string key,
        CancellationToken cancellationToken = default)
    {
        RedisValue data = await _database.StringGetAsync(key);

        if (!data.HasValue)
        {
            return default;
        }

        return JsonSerializer.Deserialize<TResponse>(data.ToString());
    }

    /// <summary>
    ///     Retrieves a cached string value by key.
    /// </summary>
    /// <param name="key">
    ///     The cache key.
    /// </param>
    /// <param name="cancellationToken">
    ///     Cancellation token (not used by Redis operations).
    /// </param>
    /// <returns>
    ///     The cached string value if found; otherwise <c>null</c>.
    /// </returns>
    public async ValueTask<string?> Get(
        string key,
        CancellationToken cancellationToken = default)
    {
        RedisValue data = await _database.StringGetAsync(key);
        return data.HasValue ? data.ToString() : null;
    }

    /// <summary>
    ///     Stores a value in the cache using JSON serialization.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value to cache.
    /// </typeparam>
    /// <param name="key">
    ///     The cache key.
    /// </param>
    /// <param name="data">
    ///     The value to cache.
    /// </param>
    /// <param name="expiration">
    ///     Optional expiration time. If not specified,
    ///     the default expiration will be applied.
    /// </param>
    /// <param name="cancellationToken">
    ///     Cancellation token (not used by Redis operations).
    /// </param>
    public ValueTask Set<T>(
        string key,
        T data,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(data);
        return Set(key, json, expiration, cancellationToken);
    }

    /// <summary>
    ///     Stores an object in the cache using JSON serialization.
    /// </summary>
    /// <param name="key">
    ///     The cache key.
    /// </param>
    /// <param name="data">
    ///     The object to cache.
    /// </param>
    /// <param name="expiration">
    ///     Optional expiration time. If not specified,
    ///     the default expiration will be applied.
    /// </param>
    /// <param name="cancellationToken">
    ///     Cancellation token (not used by Redis operations).
    /// </param>
    public ValueTask Set(
        string key,
        object data,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(data);
        return Set(key, json, expiration, cancellationToken);
    }

    /// <summary>
    ///     Stores a string value in the cache.
    /// </summary>
    /// <param name="key">
    ///     The cache key.
    /// </param>
    /// <param name="data">
    ///     The string value to cache.
    /// </param>
    /// <param name="expiration">
    ///     Optional expiration time. If not specified,
    ///     the default expiration will be applied.
    /// </param>
    /// <param name="cancellationToken">
    ///     Cancellation token (not used by Redis operations).
    /// </param>
    public async ValueTask Set(
        string key,
        string data,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        TimeSpan exp = expiration ?? DefaultExpiration;
        await _database.StringSetAsync(key, data, exp);
    }

    /// <summary>
    ///     Removes a cached value by key.
    /// </summary>
    /// <param name="key">
    ///     The cache key.
    /// </param>
    /// <param name="cancellationToken">
    ///     Cancellation token (not used by Redis operations).
    /// </param>
    public async ValueTask Remove(
        string key,
        CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync(key);
    }
}
