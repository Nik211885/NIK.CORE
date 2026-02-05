using Microsoft.Extensions.DependencyInjection;

namespace NIK.CORE.DOMAIN.Attributes;

/// <summary>
///     Marks a class to be automatically registered
///     into the dependency injection container via reflection.
/// </summary>
/// <remarks>
///     This attribute is typically used during assembly scanning
///     to determine the service lifetime when registering dependencies.
/// </remarks>
[AttributeUsage(AttributeTargets.Interface)]
public sealed class ReflectionDependencyAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ReflectionDependencyAttribute"/> class.
    /// </summary>
    /// <param name="lifetime">
    ///     The service lifetime to use when registering the decorated class
    ///     (Singleton, Scoped, or Transient).
    /// </param>
    public ReflectionDependencyAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    /// <summary>
    ///     Gets the service lifetime associated with the decorated class.
    /// </summary>
    public ServiceLifetime Lifetime { get; }
}
