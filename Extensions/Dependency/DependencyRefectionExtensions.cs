using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NIK.CORE.DOMAIN.Attributes;
using NIK.CORE.DOMAIN.Contracts;
using NIK.CORE.DOMAIN.Implements;

namespace NIK.CORE.DOMAIN.Extensions.Dependency;

/// <summary>
/// Provides extension methods for automated service registration using Reflection and Custom Attributes.
/// </summary>
public static class DependencyInjectionExtensions
{
    
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Scans assemblies for interfaces marked with <see cref="ReflectionDependencyAttribute"/> 
        /// and registers their implementations automatically.
        /// </summary>
        /// <param name="registerFrom">The assembly containing the decorated interfaces. Defaults to the calling assembly.</param>
        /// <param name="implementationFrom">The assembly containing the concrete implementation classes. Defaults to the calling assembly.</param>
        /// <returns>The <see cref="IServiceCollection"/> for further configuration chaining.</returns>
        IServiceCollection AddReflectionServices(
            Assembly? registerFrom = null, 
            Assembly? implementationFrom = null)
        {
            registerFrom ??= Assembly.GetCallingAssembly();
            implementationFrom ??= Assembly.GetCallingAssembly();
            services.TryAddSingleton<IFactoryServices, FactoryServices>();
            var interfaceMap = registerFrom.GetTypes()
                .Where(t => t.IsInterface)
                .Select(t => new { InterfaceType = t, Attribute = t.GetCustomAttribute<ReflectionDependencyAttribute>() })
                .Where(x => x.Attribute != null)
                .ToList();
            foreach (var item in interfaceMap)
            {
                var interfaceType = item.InterfaceType;
                var lifetime = item.Attribute!.Lifetime;
                var implementations = implementationFrom.GetTypes()
                    .Where(t => t is { IsClass: true, IsAbstract: false } && interfaceType.IsAssignableFrom(t))
                    .Select(t => new
                    {
                        ImplementationType = t,
                        KeyAttribute = t.GetCustomAttribute<NameKeyDependencyAttribute>()
                    })
                    .ToList();
                switch (implementations.Count)
                {
                    case 0:
                        continue;
                    case 1:
                    {
                        var impl = implementations[0];
                        var descriptor = ServiceDescriptor.Describe(interfaceType, impl.ImplementationType, lifetime);
                        services.Add(descriptor);
                        break;
                    }
                    default:
                    {
                        foreach (var impl in implementations)
                        {
                            string key = impl.KeyAttribute?.Name ?? impl.ImplementationType.Name;
                            var descriptor = new ServiceDescriptor(interfaceType, key, impl.ImplementationType, lifetime);
                            services.TryAddEnumerable(descriptor);
                        }

                        break;
                    }
                }
            }
            return services;
        }
    }
}
