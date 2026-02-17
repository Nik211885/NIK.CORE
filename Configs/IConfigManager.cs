using Microsoft.EntityFrameworkCore;

namespace NIK.CORE.DOMAIN.Configs;

/// <summary>
/// Defines operations for managing configuration keys and their associated values.
/// Provides querying, creation, update, activation, deletion,
/// and persistence capabilities for configuration entities.
/// </summary>
public interface IConfigManager
{
    /// <summary>
    /// Gets an <see cref="IQueryable{T}"/> for querying configuration keys.
    /// </summary>
    IQueryable<ConfigKey> QueryConfigKeys { get; }

    /// <summary>
    /// Gets an <see cref="IQueryable{T}"/> for querying configuration values.
    /// </summary>
    IQueryable<ConfigValue> QueryConfigValues { get; }

    /// <summary>
    /// Creates a new configuration key.
    /// </summary>
    /// <param name="configKey">The configuration key to create.</param>
    /// <returns>The created <see cref="ConfigKey"/>.</returns>
    Task<ConfigKey> CreateConfigKeyAsync(ConfigKey configKey);

    /// <summary>
    /// Updates an existing configuration key.
    /// </summary>
    /// <param name="configKey">The configuration key to update.</param>
    /// <returns>The updated <see cref="ConfigKey"/>.</returns>
    Task<ConfigKey> UpdateConfigKeyAsync(ConfigKey configKey);

    /// <summary>
    /// Activates or deactivates a configuration key.
    /// </summary>
    /// <param name="configKey">The configuration key to modify.</param>
    /// <param name="isActive">Indicates whether the key should be active.</param>
    /// <returns>The updated <see cref="ConfigKey"/>.</returns>
    Task<ConfigKey> ActiveConfigKeyAsync(ConfigKey configKey, bool isActive);

    /// <summary>
    /// Activates or deactivates multiple configuration keys.
    /// </summary>
    /// <param name="configKeyIds">The identifiers of the configuration keys.</param>
    /// <param name="isActive">Indicates whether the keys should be active.</param>
    Task ActiveConfigsKeyAsync(List<Guid> configKeyIds, bool isActive);

    /// <summary>
    /// Deletes multiple configuration keys by their identifiers.
    /// </summary>
    /// <param name="configKeyIds">The identifiers of the configuration keys to delete.</param>
    Task DeleteConfigKeysAsync(List<Guid> configKeyIds);

    /// <summary>
    /// Creates a new configuration value under a specified configuration key.
    /// </summary>
    /// <param name="configKey">The parent configuration key.</param>
    /// <param name="configValue">The configuration value to create.</param>
    /// <returns>The created <see cref="ConfigValue"/>.</returns>
    Task<ConfigValue> CreateConfigValueAsync(ConfigKey configKey, ConfigValue configValue);

    /// <summary>
    /// Updates an existing configuration value.
    /// </summary>
    /// <param name="configValue">The configuration value to update.</param>
    /// <returns>The updated <see cref="ConfigValue"/>.</returns>
    Task<ConfigValue> UpdateConfigValueAsync(ConfigValue configValue);

    /// <summary>
    /// Activates or deactivates a configuration value.
    /// </summary>
    /// <param name="configValue">The configuration value to modify.</param>
    /// <param name="isActive">Indicates whether the value should be active.</param>
    /// <returns>The updated <see cref="ConfigValue"/>.</returns>
    Task<ConfigValue> ActiveConfigValueAsync(ConfigValue configValue, bool isActive);

    /// <summary>
    /// Activates or deactivates multiple configuration values.
    /// </summary>
    /// <param name="configValueIds">The identifiers of the configuration values.</param>
    /// <param name="isActive">Indicates whether the values should be active.</param>
    Task ActiveConfigsValueAsync(List<Guid> configValueIds, bool isActive);

    /// <summary>
    /// Deletes multiple configuration values by their identifiers.
    /// </summary>
    /// <param name="configValueIds">The identifiers of the configuration values to delete.</param>
    Task DeleteConfigsValueAsync(List<Guid> configValueIds);

    /// <summary>
    /// Retrieves a configuration key by its unique identifier.
    /// </summary>
    /// <param name="configKeyId">The configuration key identifier.</param>
    /// <returns>
    /// The matching <see cref="ConfigKey"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<ConfigKey?> GetConfigKeyByIdAsync(Guid configKeyId);

    /// <summary>
    /// Retrieves a configuration key by its key name.
    /// </summary>
    /// <param name="key">The key name.</param>
    /// <returns>
    /// The matching <see cref="ConfigKey"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<ConfigKey?> GetConfigKeyByKeyAsync(string key);

    /// <summary>
    /// Retrieves all configuration keys.
    /// </summary>
    /// <param name="include">
    /// Indicates whether related configuration values should be included.
    /// </param>
    /// <returns>
    /// A read-only collection of <see cref="ConfigKey"/>.
    /// </returns>
    Task<IReadOnlyCollection<ConfigKey>> GetAllConfigKeysAsync(bool include = false);

    /// <summary>
    /// Persists all changes made in the current unit of work to the underlying data store.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
