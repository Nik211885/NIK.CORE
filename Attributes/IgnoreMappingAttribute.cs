namespace NIK.CORE.DOMAIN.Attributes;

/// <summary>
/// Marks a property to be ignored during automatic object or query mapping.
/// </summary>
/// <remarks>
/// Commonly used with reflection or expression-based mappers
/// to prevent mapping of audit, key, or computed fields.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnoreMappingAttribute : Attribute;
