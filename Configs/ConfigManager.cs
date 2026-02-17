using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NIK.CORE.DOMAIN.Abstracts;
using NIK.CORE.DOMAIN.Exceptions;
using NIK.CORE.DOMAIN.Extensions.DataTypes;

namespace NIK.CORE.DOMAIN.Configs;

/// <summary>
/// Provides application-level management for <see cref="ConfigKey"/> and <see cref="ConfigValue"/>.
/// </summary>
/// <remarks>
/// Design principles:
/// <list type="bullet">
/// <item>
/// All keys and values are normalized using <c>ToLowerInvariant()</c>
/// to enforce case-insensitive uniqueness.
/// </item>
/// <item>
/// Normalization avoids database collation dependency and keeps queries index-friendly.
/// </item>
/// <item>
/// Duplicate validation is performed at application level. 
/// A unique index at database level is strongly recommended for concurrency safety.
/// </item>
/// </list>
/// </remarks>
public class ConfigManager : IConfigManager
{
    private readonly ILogger<ConfigManager> _logger;
    private readonly BaseDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of <see cref="ConfigManager"/>.
    /// </summary>
    public ConfigManager(ILogger<ConfigManager> logger, BaseDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Provides query access to configuration keys.
    /// </summary>
    public IQueryable<ConfigKey> QueryConfigKeys => _dbContext.ConfigKeys.AsQueryable();

    /// <summary>
    /// Provides query access to configuration values.
    /// </summary>
    public IQueryable<ConfigValue> QueryConfigValues => _dbContext.ConfigValues.AsQueryable();

    /// <summary>
    /// Creates a new configuration key.
    /// </summary>
    /// <param name="configKey">The configuration key entity.</param>
    /// <returns>The created <see cref="ConfigKey"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configKey"/> is null.</exception>
    /// <exception cref="BadRequestException">Thrown when a duplicate key exists.</exception>
    /// <remarks>
    /// The key is normalized to lowercase before persistence.
    /// </remarks>
    public async Task<ConfigKey> CreateConfigKeyAsync(ConfigKey configKey)
    {
        ArgumentNullException.ThrowIfNull(configKey);

        configKey.Key = configKey.Key.ToLowerInvariant();

        var exitsConfigKey = await _dbContext.ConfigKeys
            .FirstOrDefaultAsync(x => x.Key == configKey.Key);

        if (exitsConfigKey is not null)
        {
            ThrowHelper.ThrowBadRequest(CoreMessages.ExitConfigKey);
        }

        _dbContext.ConfigKeys.Add(configKey);
        await _dbContext.SaveChangesAsync();
        return configKey;
    }

    /// <summary>
    /// Updates an existing configuration key.
    /// </summary>
    /// <param name="configKey">The updated configuration key entity.</param>
    /// <returns>The updated <see cref="ConfigKey"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="BadRequestException">
    /// Thrown when the key does not exist or a duplicate key is detected.
    /// </exception>
    /// <remarks>
    /// The key value is normalized before duplicate validation.
    /// </remarks>
    public async Task<ConfigKey> UpdateConfigKeyAsync(ConfigKey configKey)
    {
        ArgumentNullException.ThrowIfNull(configKey);

        configKey.Key = configKey.Key.ToLowerInvariant();

        var duplicate = await _dbContext.ConfigKeys
            .AnyAsync(x => x.Id != configKey.Id && x.Key == configKey.Key);

        if (duplicate)
        {
            ThrowHelper.ThrowBadRequest(CoreMessages.ExitConfigKey);
        }

        var exitsConfigKey = await _dbContext.ConfigKeys
            .FirstOrDefaultAsync(x => x.Id == configKey.Id);

        if (exitsConfigKey is null)
        {
            ThrowHelper.ThrowBadRequest(CoreMessages.NotExitConfigKey);
        }

        exitsConfigKey.Key = configKey.Key;
        exitsConfigKey.Description = configKey.Description;
        exitsConfigKey.IsActive = configKey.IsActive;
        exitsConfigKey.UpdatedBy = configKey.UpdatedBy;
        exitsConfigKey.UpdatedOnUtc = configKey.UpdatedOnUtc;

        _dbContext.ConfigKeys.Update(exitsConfigKey);
        await _dbContext.SaveChangesAsync();
        return exitsConfigKey;
    }

    /// <summary>
    /// Activates or deactivates a configuration key.
    /// </summary>
    /// <param name="configKey">The target configuration key.</param>
    /// <param name="isActive">Activation state.</param>
    /// <returns>The updated <see cref="ConfigKey"/>.</returns>
    public async Task<ConfigKey> ActiveConfigKeyAsync(ConfigKey configKey, bool isActive)
    {
        ArgumentNullException.ThrowIfNull(configKey);

        configKey.IsActive = isActive;
        _dbContext.ConfigKeys.Update(configKey);
        await _dbContext.SaveChangesAsync();
        return configKey;
    }

    /// <summary>
    /// Bulk activation/deactivation for configuration keys.
    /// </summary>
    public Task ActiveConfigsKeyAsync(List<Guid> configKeyIds, bool isActive)
    {
        return _dbContext.ConfigKeys
            .Where(x => configKeyIds.Contains(x.Id))
            .ExecuteUpdateAsync(x =>
                x.SetProperty(value => value.IsActive, isActive));
    }

    /// <summary>
    /// Deletes multiple configuration keys.
    /// </summary>
    public Task DeleteConfigKeysAsync(List<Guid> configKeyIds)
    {
        return _dbContext.ConfigKeys
            .Where(x => configKeyIds.Contains(x.Id))
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Creates a new configuration value under a specific configuration key.
    /// </summary>
    /// <remarks>
    /// Value is normalized to lowercase before persistence.
    /// Uniqueness is validated within the same <see cref="ConfigKey"/>.
    /// </remarks>
    public async Task<ConfigValue> CreateConfigValueAsync(ConfigKey configKey, ConfigValue configValue)
    {
        ArgumentNullException.ThrowIfNull(configKey);
        ArgumentNullException.ThrowIfNull(configValue);

        configValue.Value = configValue.Value.ToLowerInvariant();

        var configValueExits = await _dbContext.ConfigValues
            .Where(x => x.ConfigKeyId == configKey.Id &&
                        x.Value == configValue.Value)
            .FirstOrDefaultAsync();

        if (configValueExits is not null)
        {
            ThrowHelper.ThrowBadRequest(CoreMessages.ExitConfigValue);
        }

        configValue.ConfigKey = configKey;
        configValue.ConfigKeyId = configKey.Id;

        _dbContext.ConfigValues.Add(configValue);
        await _dbContext.SaveChangesAsync();
        return configValue;
    }

    /// <summary>
    /// Updates an existing configuration value.
    /// </summary>
    /// <remarks>
    /// Value is normalized before duplicate validation.
    /// </remarks>
    public async Task<ConfigValue> UpdateConfigValueAsync(ConfigValue configValue)
    {
        ArgumentNullException.ThrowIfNull(configValue);

        configValue.Value = configValue.Value.ToLowerInvariant();

        var duplicate = await _dbContext.ConfigValues
            .AnyAsync(x => x.Id != configValue.Id &&
                           x.ConfigKeyId == configValue.ConfigKeyId &&
                           x.Value == configValue.Value);

        if (duplicate)
        {
            ThrowHelper.ThrowBadRequest(CoreMessages.ExitConfigValue);
        }

        _dbContext.ConfigValues.Update(configValue);
        await _dbContext.SaveChangesAsync();
        return configValue;
    }

    /// <summary>
    /// Activates or deactivates a configuration value.
    /// </summary>
    public async Task<ConfigValue> ActiveConfigValueAsync(ConfigValue configValue, bool isActive)
    {
        ArgumentNullException.ThrowIfNull(configValue);

        configValue.IsActive = isActive;
        _dbContext.ConfigValues.Update(configValue);
        await _dbContext.SaveChangesAsync();
        return configValue;
    }

    /// <summary>
    /// Bulk activation/deactivation for configuration values.
    /// </summary>
    public Task ActiveConfigsValueAsync(List<Guid> configValueIds, bool isActive)
    {
        return _dbContext.ConfigValues
            .Where(x => configValueIds.Contains(x.Id))
            .ExecuteUpdateAsync(x =>
                x.SetProperty(value => value.IsActive, isActive));
    }

    /// <summary>
    /// Deletes multiple configuration values.
    /// </summary>
    public Task DeleteConfigsValueAsync(List<Guid> configValueIds)
    {
        return _dbContext.ConfigValues
            .Where(x => configValueIds.Contains(x.Id))
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Retrieves an active configuration key by identifier.
    /// </summary>
    public Task<ConfigKey?> GetConfigKeyByIdAsync(Guid configKeyId)
    {
        return _dbContext.ConfigKeys
            .Where(x => x.Id == configKeyId && x.IsActive)
            .Include(x => x.Values)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves an active configuration key by its key string.
    /// </summary>
    /// <remarks>
    /// Input key is normalized before comparison.
    /// </remarks>
    public Task<ConfigKey?> GetConfigKeyByKeyAsync(string key)
    {
        return _dbContext.ConfigKeys
            .Where(x => x.Key == key.ToLowerInvariant())
            .Where(x => x.IsActive)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves all active configuration keys.
    /// </summary>
    /// <param name="include">Indicates whether to include related values.</param>
    public async Task<IReadOnlyCollection<ConfigKey>> GetAllConfigKeysAsync(bool include = false)
    {
        var configKeyQueryable = _dbContext.ConfigKeys.Where(x => x.IsActive);

        if (include)
        {
            configKeyQueryable = configKeyQueryable.Include(x => x.Values);
        }

        return await configKeyQueryable
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    public Task<int> SaveChangesAsync(CancellationToken cancellation = default)
    {
        return _dbContext.SaveChangesAsync(cancellation);
    }
}
