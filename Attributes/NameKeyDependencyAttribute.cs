namespace NIK.CORE.DOMAIN.Attributes;

/// <summary>
///     Specifies a named key for dependency injection registration.
///     <para>
///     This attribute is used to associate an implementation class
///     with a specific name, allowing it to be resolved later via
///     keyed or named dependency resolution (e.g. factories).
///     </para>
/// </summary>
/// <remarks>
///     Commonly used when multiple implementations of the same
///     service interface exist and must be distinguished by name,
///     such as caching providers, payment providers, or storage engines.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class NameKeyDependencyAttribute : Attribute
{
    /// <summary>
    ///     Gets the name key associated with the dependency.
    /// </summary>
    public string Name { get; }

    /// <param name="name">
    ///     The unique name used to identify the dependency implementation.
    /// </param>
    public NameKeyDependencyAttribute(string name)
    {
        Name = name;
    }
}
