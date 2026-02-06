using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.Validator;

namespace NIK.CORE.DOMAIN.Extensions.Dependency;

/// <summary>
/// Provides dependency injection extensions for registering validators.
/// </summary>
public static class DependencyValidatorsExtensions
{
    /// <summary>
    /// Service collection validation extensions.
    /// </summary>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers all implementations of <see cref="IValidator{T}"/> found in the specified assembly.
        /// </summary>
        /// <param name="assembly">
        /// The assembly to scan for validator implementations.
        /// If <c>null</c>, the executing assembly is used.
        /// </param>
        /// <returns>
        /// The same <see cref="IServiceCollection"/> instance for chaining.
        /// </returns>
        public IServiceCollection AddValidatorFromAssembly(Assembly? assembly)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            var validatorTypes = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { Implementation = t, Interface = i })
                .Where(x => x.Interface.IsGenericType &&
                            x.Interface.GetGenericTypeDefinition() == typeof(IValidator<>));

            foreach (var item in validatorTypes)
            {
                services.AddScoped(item.Interface, item.Implementation);
            }

            return services;
        }
    }
}
