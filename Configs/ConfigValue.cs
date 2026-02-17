using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NIK.CORE.DOMAIN.Configs;

[Table("ConfigValues")]
public class ConfigValue
{
    /// <summary>
    /// Gets or sets the unique identifier of the configuration value.
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the related configuration key.
    /// </summary>
    /// <remarks>
    /// Links this configuration value to its definition in <see cref="ConfigKey"/>.
    /// </remarks>
    [Required]
    public Guid ConfigKeyId { get; set; }
    /// <summary>
    /// Gets or sets the actual configuration value.
    /// </summary>
    /// <remarks>
    /// Stored as a string to support flexible data types
    /// such as JSON, numbers, booleans, or connection strings.
    /// </remarks>
    [Required]
    public string Value { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the environment in which this configuration value applies.
    /// </summary>
    /// <remarks>
    /// Determines whether the value is used in Development or Production.
    /// </remarks>
    public ConfigEnvironment Environment { get; set; } = ConfigEnvironment.Production;
    /// <summary>
    /// Gets or sets the UTC date and time when the configuration value was created.
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
    /// Gets or sets the UTC date and time when the configuration value was last modified.
    /// </summary>
    public DateTime? UpdatedOnUtc { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the user or system that last modified this record.
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// Gets or sets the navigation property to the related configuration key.
    /// </summary>
    [ForeignKey(nameof(ConfigKeyId))]
    public ConfigKey ConfigKey { get; set; } = null!;
    /// <summary>
    /// 
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Represents the application environment in which a configuration value is applied.
/// </summary>
/// <remarks>
/// This enum is intentionally kept simple and supports only two environments:
/// Development and Production.
/// </remarks>
public enum ConfigEnvironment
{
    /// <summary>
    /// Development environment.
    /// Used for local development and testing.
    /// </summary>
    Development = 0,
    /// <summary>
    /// Production environment.
    /// Used in live and deployed systems.
    /// </summary>
    Production = 1
}
