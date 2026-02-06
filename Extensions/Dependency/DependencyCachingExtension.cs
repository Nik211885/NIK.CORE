using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.Caching.ConfigModels;
using NIK.CORE.DOMAIN.Caching.Contracts;
using NIK.CORE.DOMAIN.Caching.Enums;
using NIK.CORE.DOMAIN.Caching.Implements; 
using StackExchange.Redis;

namespace NIK.CORE.DOMAIN.Extensions.Dependency;

/// <summary>
///     Provides extension methods for registering caching services
///     into the dependency injection container.
/// </summary>
public static class DependencyCachingExtension
{
    /// <summary>
    ///     Defines extension members for <see cref="IServiceCollection"/>
    ///     related to caching registration.
    /// </summary>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        ///     Registers the default in-memory caching implementation.
        /// </summary>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance.
        /// </returns>
        IServiceCollection AddMemoryCaching()
        {
            serviceCollection.AddSingleton<ICache, MemoryCache>();
            return serviceCollection;
        }

        /// <summary>
        ///     Registers Redis caching using a configuration delegate.
        /// </summary>
        /// <param name="redisConfig">
        ///     An action used to configure <see cref="RedisConfigs"/>.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance.
        /// </returns>
        IServiceCollection AddRedisCache(Action<RedisConfigs> redisConfig)
        {
            var config = new RedisConfigs();
            redisConfig.Invoke(config);
            return serviceCollection.AddRedisCache(config);
        }

        /// <summary>
        ///     Registers Redis as the caching provider using
        ///     the specified Redis configuration.
        /// </summary>
        /// <param name="redisConfig">
        ///     The Redis caching configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance.
        /// </returns>
        IServiceCollection AddRedisCache(RedisConfigs redisConfig)
        {
            serviceCollection.AddSingleton<ICache, RedisCache>();
            serviceCollection.AddRedisConnectionMultiplexer(redisConfig);
            return serviceCollection;
        }

        /// <summary>
        ///     Registers caching services based on the provided configuration.
        ///     <para>
        ///     If a single caching configuration is provided, it will be
        ///     registered as the default <see cref="ICache"/>.
        ///     </para>
        ///     <para>
        ///     If multiple caching configurations are provided, keyed
        ///     dependency injection will be used and an
        ///     <see cref="ICacheFactory"/> will be registered.
        ///     </para>
        /// </summary>
        /// <param name="action">
        ///     A configuration action used to specify caching providers.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance.
        /// </returns>
        IServiceCollection AddCaching(Action<ActionCachingConfig> action)
        {
            var config = new ActionCachingConfig();
            action.Invoke(config);
            if (config.CachingConfigs.Count == 0)
            {
                throw new InvalidOperationException(
                    "No caching configuration was provided.");
            }
            if (config.CachingConfigs.Count == 1)
            {
                serviceCollection.RegisterSingleCache(config.CachingConfigs.First());
                return serviceCollection;
            }
            foreach (var cachingConfig in config.CachingConfigs)
            {
                serviceCollection.RegisterKeyedCache(cachingConfig);
            }
            serviceCollection.AddSingleton<ICacheFactory, CacheFactory>();
            return serviceCollection;
        }

        /// <summary>
        ///     Registers a single caching provider as the default cache.
        /// </summary>
        private void RegisterSingleCache(ICachingConfig config)
        {
            switch (config)
            {
                case MemoryCachingConfig:
                    serviceCollection.AddMemoryCaching();
                    break;
                case RedisConfigs redisConfigs:
                    serviceCollection.AddRedisCache(redisConfigs);
                    break;
                default:
                    throw new NotSupportedException(
                        "The specified caching provider is not supported.");
            }
        }

        /// <summary>
        ///     Registers a caching provider using keyed dependency injection.
        /// </summary>
        private void RegisterKeyedCache(ICachingConfig config)
        {
            switch (config)
            {
                case MemoryCachingConfig:
                    serviceCollection.AddKeyedSingleton<ICache, MemoryCache>(
                        nameof(CachingProvider.InMemory));
                    break;
                case RedisConfigs redisConfigs:
                    serviceCollection.AddRedisConnectionMultiplexer(redisConfigs);
                    serviceCollection.AddKeyedSingleton<ICache, RedisCache>(
                        nameof(CachingProvider.Redis));
                    break;
                default:
                    throw new NotSupportedException(
                        "The specified caching provider is not supported.");
            }
        }

        /// <summary>
        ///     Registers a Redis <see cref="IConnectionMultiplexer"/>
        ///     using the provided Redis configuration.
        /// </summary>
        private IServiceCollection AddRedisConnectionMultiplexer(RedisConfigs conf)
        {
            var options = new ConfigurationOptions
            {
                AbortOnConnectFail = conf.AbortOnConnectFail,
                ConnectRetry = conf.ConnectRetry,
                ConnectTimeout = conf.ConnectTimeout,
                SyncTimeout = conf.SyncTimeout
            };
            foreach (var redis in conf.RedisConfigurations)
            {
                options.EndPoints.Add(redis.Endpoint, redis.Port);
                if (!string.IsNullOrEmpty(redis.Password))
                {
                    options.User = redis.UserName;
                    options.Password = redis.Password;
                }
                options.Ssl = redis.Ssl;
            }
            serviceCollection.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(options));
            return serviceCollection;
        }
    }
}

/// <summary>
///     Represents configuration options for caching registration.
/// </summary>
public class ActionCachingConfig
{
    /// <summary>
    ///     Gets the list of caching configurations to be registered.
    /// </summary>
    public List<ICachingConfig> CachingConfigs { get; } = [];

    /// <summary>
    ///     Adds one or more caching configurations.
    /// </summary>
    /// <param name="cachingConfigs">
    ///     The caching configurations to register.
    /// </param>
    public void AddProvider(params ICachingConfig[] cachingConfigs)
    {
        CachingConfigs.AddRange(cachingConfigs);
    }
}
