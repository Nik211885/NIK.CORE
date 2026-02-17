using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NIK.CORE.DOMAIN.Configs;

/// <summary>
/// Represents a configuration key definition.
/// </summary>
/// <remarks>
/// Configuration keys define available configuration entries.
/// Each key can have one or more associated values depending on scope
/// (environment, tenant, application, etc.).
/// </remarks>
[Table("ConfigKeys")]
public class ConfigKey
{
    /// <summary>
    /// Gets the unique identifier of the configuration key.
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    /// <summary>
    /// Gets the unique key name.
    /// </summary>
    /// <remarks>
    /// This value should be unique across the system.
    /// Example: "MaxLoginAttempts", "EnableFeatureX"
    /// </remarks>
    [Required]
    [MaxLength(200)]
    public string Key { get; set; } = string.Empty;
    /// <summary>
    ///  Name for config key
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets the human-readable description of the configuration key.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    /// <summary>
    /// Gets the UTC timestamp when the configuration key was created.
    /// </summary>
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Gets or sets the identifier of the user or system that created this record.
    /// </summary>
    /// <remarks>
    /// Can be a user ID, service name, or system identifier.
    /// </remarks>
    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    /// <summary>
    /// Gets the UTC timestamp when the configuration key was last updated.
    /// </summary>
    public DateTime? UpdatedOnUtc { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the user or system that last modified this record.
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the configuration key is active.
    /// </summary>
    /// <remarks>
    /// Inactive configuration keys are ignored by the system and should not
    /// be used when resolving configuration values.
    /// </remarks>
    public bool IsActive { get; set; } = true;
    /// <summary>
    /// 
    /// </summary>
    public ConfigType ConfigType { get; set; } = ConfigType.Custom;
    /// <summary>
    /// Gets the collection of configuration values associated with this key.
    /// </summary>
    public ICollection<ConfigValue> Values { get; set; } = new List<ConfigValue>();
}

public enum ConfigType
{
    System = 0,
    Custom = 1
}
