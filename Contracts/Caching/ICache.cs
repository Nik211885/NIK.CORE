namespace NIK.CORE.DOMAIN.Contracts.Caching;

/// <summary>
///     Defines a contract for a cache provider that supports
///     storing, retrieving, and removing cached data.
/// </summary>
public interface ICache
{
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
    ///     and contains the cached value.
    /// </returns>
    ValueTask<TResponse?> Get<TResponse>(string key, CancellationToken cancellationToken = default);
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
    ///     and contains the cached string value.
    /// </returns>
    ValueTask<string?> Get( string key, CancellationToken cancellationToken = default);
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
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    ValueTask Set<T>( string key, T data, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
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
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    ValueTask Set( string key, object data, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

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
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    ValueTask Set( string key, string data, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    /// <summary>
    ///     Removes the cached value associated with the specified key.
    /// </summary>
    /// <param name="key">
    ///     The unique key identifying the cached value.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    ValueTask Remove( string key, CancellationToken cancellationToken = default);
}