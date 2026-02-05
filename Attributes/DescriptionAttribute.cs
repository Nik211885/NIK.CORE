namespace NIK.CORE.DOMAIN.Attributes;

/// <summary>
/// Specifies metadata for enum fields.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public sealed class EnumMetadataAttribute(string description) : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    public string Description { get; } = description;
}
