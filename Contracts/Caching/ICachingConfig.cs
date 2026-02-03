namespace NIK.CORE.DOMAIN.Contracts.Caching;

/// <summary>
///     Represents a base contract for caching configuration objects.
///     <para>
///     Implementations of this interface are used to describe
///     configuration settings for different caching providers
///     (e.g. in-memory cache, Redis cache).
///     </para>
///     <para>
///     This interface serves as a marker and common abstraction
///     to allow registering and resolving caching providers
///     in a flexible and extensible way.
///     </para>
/// </summary>
public interface ICachingConfig;