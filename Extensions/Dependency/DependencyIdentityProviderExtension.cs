using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.Contracts;
using NIK.CORE.DOMAIN.Implements;

namespace NIK.CORE.DOMAIN.Extensions.Dependency;

/// <summary>
///     Provides extension methods for registering identity-related services
///     into the dependency injection container.
/// </summary>
public static class DependencyIdentityProviderExtension
{
    /// <summary>
    ///     Defines extension members for <see cref="IServiceCollection"/>
    ///     used to register identity provider services.
    /// </summary>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        ///     Registers the default identity provider implementation.
        ///     <para>
        ///     This method registers <see cref="IHttpContextAccessor"/>
        ///     and binds <see cref="IIdentityProvider"/> to its concrete
        ///     implementation, enabling access to the current user's
        ///     identity, roles, permissions, and claims.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance
        ///     to allow fluent chaining.
        /// </returns>
        public IServiceCollection AddIdentityProvider()
        {
            serviceCollection.AddHttpContextAccessor();
            serviceCollection.AddSingleton<IIdentityProvider, IdentityProvider>();
            return serviceCollection;
        }
    }
}
