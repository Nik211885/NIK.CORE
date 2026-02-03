using NIK.CORE.DOMAIN.Contracts.Caching;

namespace NIK.CORE.DOMAIN.Models.Configs.Caching;

/// <summary>
///     Represents advanced configuration settings for Redis caching,
///     including connection behavior and cluster-specific configurations.
/// </summary>
public class RedisConfigs : ICachingConfig
{
    /// <summary>
    ///     Gets or sets a value indicating whether the connection attempt
    ///     should fail immediately when the Redis server is unavailable.
    ///     <para>
    ///     Default value is <c>false</c>.
    ///     </para>
    /// </summary>
    public bool AbortOnConnectFail { get; set; } = false;

    /// <summary>
    ///     Gets or sets the number of retry attempts when establishing
    ///     a connection to the Redis server.
    ///     <para>
    ///     Default value is <c>5</c>.
    ///     </para>
    /// </summary>
    public int ConnectRetry { get; set; } = 5;

    /// <summary>
    ///     Gets or sets the connection timeout, in milliseconds.
    ///     <para>
    ///     Default value is <c>5000</c> milliseconds.
    ///     </para>
    /// </summary>
    public int ConnectTimeout { get; set; } = 5000;

    /// <summary>
    ///     Gets or sets the synchronous operation timeout, in milliseconds.
    ///     <para>
    ///     Default value is <c>5000</c> milliseconds.
    ///     </para>
    /// </summary>
    public int SyncTimeout { get; set; } = 5000;

    /// <summary>
    ///     Gets or sets the list of Redis configurations for each cluster
    ///     or endpoint.
    /// </summary>
    public List<RedisConfig> RedisConfigurations { get; } = [];
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configs"></param>
    public void AddRedisConfiguration(params RedisConfig[] configs)
    {
        RedisConfigurations.AddRange(configs);
    }
}


/// <summary>
///     Represents configuration settings for a Redis caching provider.
/// </summary>
public class RedisConfig
{
    /// <summary>
    ///     Gets or sets the Redis server endpoint.
    ///     <para>
    ///     Default value is <c>localhost</c>.
    ///     </para>
    /// </summary>
    public string Endpoint { get; set; } = "localhost";

    /// <summary>
    ///     Gets or sets the port number used to connect to the Redis server.
    ///     <para>
    ///     Default value is <c>6379</c>.
    ///     </para>
    /// </summary>
    public int Port { get; set; } = 6379;

    /// <summary>
    ///     Gets or sets the username used for Redis authentication.
    ///     <para>
    ///     This property is optional and depends on the Redis configuration.
    ///     </para>
    /// </summary>
    public string UserName { get; set; } = "redis";

    /// <summary>
    ///     Gets or sets the password used for Redis authentication.
    ///     <para>
    ///     Leave this value empty if authentication is not required.
    ///     </para>
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets a value indicating whether SSL should be used
    ///     when connecting to the Redis server.
    /// </summary>
    public bool Ssl { get; set; } = false;
}
