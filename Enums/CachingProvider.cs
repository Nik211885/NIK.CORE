namespace NIK.CORE.DOMAIN.Enums;

/// <summary>
///     Specifies the supported cache provider types.
/// </summary>
public enum CachingProvider
{
    /// <summary>
    ///     Uses an in-memory cache provider that stores data
    ///     in the application's memory.
    /// </summary>
    InMemory,

    /// <summary>
    ///     Uses a Redis-based distributed cache provider.
    /// </summary>
    Redis
}