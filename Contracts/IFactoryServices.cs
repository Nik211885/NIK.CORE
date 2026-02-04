namespace NIK.CORE.DOMAIN.Contracts;

/// <summary>
///     Defines a factory contract for creating service instances
///     identified by a string key.
/// </summary>
public interface IFactoryServices
{
    /// <summary>
    ///     Creates a service instance associated with the specified name.
    /// </summary>
    /// <param name="name">
    ///     The name key used to identify the service implementation.
    /// </param>
    /// <returns>
    ///     An instance of <typeparamref name="T"/> corresponding to the given name.
    /// </returns>
    T Create<T>(string name);
}
