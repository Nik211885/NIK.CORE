using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.Contracts.CommandQueryResponsibilitySegregation;
using NIK.CORE.DOMAIN.Implements.CommandQueryResponsibilitySegregation;

namespace NIK.CORE.DOMAIN.Extensions.Dependency;

/// <summary>
///     Provides extension methods for registering CQRS and Mediator-related services
///     into the dependency injection container.
///     <para>
///     This includes:
///     <list type="bullet">
///         <item>Command and query handlers</item>
///         <item>Domain event handlers</item>
///         <item>Mediator implementation</item>
///     </list>
///     </para>
/// </summary>
public static class DependencyCommandQueryResponsibilitySegregationExtension
{
    /// <summary>
    ///     Defines extension members for <see cref="IServiceCollection"/>
    ///     related to CQRS and Mediator registration.
    /// </summary>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        ///     Registers Mediator, command/query handlers, and domain event handlers.
        ///     <para>
        ///     This method scans the provided assembly (or the calling assembly by default)
        ///     to automatically discover and register implementations of:
        ///     <list type="bullet">
        ///         <item><see cref="IBaseHandler{TRequest, TResponse}"/></item>
        ///         <item><see cref="IDomainEventHandler{TEvent}"/></item>
        ///     </list>
        ///     </para>
        /// </summary>
        /// <param name="assembly">
        ///     The assembly to scan for handler implementations.
        ///     If null, the calling assembly will be used.
        /// </param>
        /// <param name="lifetime">
        ///     The service lifetime to apply when registering handlers.
        ///     Default is <see cref="ServiceLifetime.Scoped"/>.
        /// </param>
        /// <returns>The current <see cref="IServiceCollection"/> instance.</returns>
        IServiceCollection AddMediator( Assembly? assembly = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            assembly ??= Assembly.GetCallingAssembly();
            serviceCollection.AddMediatorHandler(lifetime);
            serviceCollection.AddHandlers(assembly, lifetime);
            serviceCollection.AddDomainEvents(assembly, lifetime);
            return serviceCollection;
        }
        /// <summary>
        ///     Registers all command and query handlers found in the specified assembly.
        ///     <para>
        ///     Scans for implementations of <see cref="IBaseHandler{TRequest, TResponse}"/>
        ///     and registers them into the DI container.
        ///     </para>
        /// </summary>
        /// <param name="assembly">
        ///     The assembly to scan. If null, the calling assembly is used.
        /// </param>
        /// <param name="lifetime">
        ///     The service lifetime of the registered handlers.
        /// </param>
        /// <returns>The current <see cref="IServiceCollection"/> instance.</returns>
        IServiceCollection AddHandlers( Assembly? assembly = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            assembly ??= Assembly.GetCallingAssembly();
            serviceCollection.AddImplementationByInterfaceFromScanAssembly( typeof(IBaseHandler<,>), assembly, lifetime);
            return serviceCollection;
        }
        /// <summary>
        ///     Registers all domain event handlers found in the specified assembly.
        ///     <para>
        ///     Scans for implementations of <see cref="IDomainEventHandler{TEvent}"/>
        ///     and registers them into the DI container.
        ///     </para>
        /// </summary>
        /// <param name="assembly">
        ///     The assembly to scan. If null, the calling assembly is used.
        /// </param>
        /// <param name="lifetime">
        ///     The service lifetime of the registered handlers.
        /// </param>
        /// <returns>The current <see cref="IServiceCollection"/> instance.</returns>
        IServiceCollection AddDomainEvents(Assembly? assembly = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            assembly ??= Assembly.GetCallingAssembly();
            serviceCollection.AddImplementationByInterfaceFromScanAssembly( typeof(IDomainEventHandler<>), assembly, lifetime);
            return serviceCollection;
        }
        /// <summary>
        ///     Registers the default <see cref="IMediator"/> implementation.
        /// </summary>
        /// <param name="lifetime">
        ///     The lifetime of the mediator service.
        ///     Default is <see cref="ServiceLifetime.Scoped"/>.
        /// </param>
        /// <returns>The current <see cref="IServiceCollection"/> instance.</returns>
        IServiceCollection AddMediatorHandler(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var descriptor = new ServiceDescriptor( typeof(IMediator), typeof(Mediator), lifetime);
            serviceCollection.Add(descriptor);
            return serviceCollection;
        }
        /// <summary>
        ///     Registers integration event handlers into the dependency
        ///     injection container by scanning the specified assembly.
        ///     <para>
        ///     All concrete classes that implement
        ///     <see cref="IIntegrationEventHandler{TEvent}"/> will be
        ///     automatically registered.
        ///     </para>
        ///     <para>
        ///     Integration events are typically used to communicate
        ///     across bounded contexts or external systems
        ///     (for example: via message broker, outbox, or event bus).
        ///     </para>
        /// </summary>
        /// <param name="assembly">
        ///     The assembly to scan for integration event handler implementations.
        ///     <para>
        ///     If not provided, the calling assembly will be used.
        ///     </para>
        /// </param>
        /// <param name="lifetime">
        ///     The service lifetime for registered integration event handlers.
        ///     <para>
        ///     Default is <see cref="ServiceLifetime.Scoped"/>.
        ///     </para>
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance
        ///     to allow fluent chaining.
        /// </returns>
        IServiceCollection AddIntegrationEvents(
            Assembly? assembly = null,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            serviceCollection.AddImplementationByInterfaceFromScanAssembly(
                typeof(IIntegrationEventHandler<>),
                assembly,
                lifetime);

            return serviceCollection;
        }
        /// <summary>
        ///     Registers one or more pipeline behaviors into the dependency
        ///     injection container.
        ///     <para>
        ///     Each provided type must be a concrete (non-abstract) class that
        ///     implements <see cref="IPipelineBehavior{TRequest, TResponse}"/>.
        ///     </para>
        /// </summary>
        /// <param name="pipeLines">
        ///     The pipeline behavior types to register.
        ///     <para>
        ///     These types must be open generic pipeline implementations
        ///     (for example: <c>ValidationPipeline&lt;,&gt;</c>).
        ///     </para>
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance
        ///     to allow fluent chaining.
        /// </returns>
        IServiceCollection AddPipelineBehaviors(params Type[] pipeLines)
        {
            foreach (var p in pipeLines)
            {
                serviceCollection.AddScoped(typeof(IPipelineBehavior<,>), p);
            }
            return serviceCollection;
        }
        /// <summary>
        ///     Scans an assembly and registers all concrete implementations
        ///     of a given open generic interface.
        ///     <para>
        ///     Example supported interfaces:
        ///     <list type="bullet">
        ///         <item><see cref="IBaseHandler{TRequest, TResponse}"/></item>
        ///         <item><see cref="IDomainEventHandler{TEvent}"/></item>
        ///     </list>
        ///     </para>
        /// </summary>
        /// <param name="scanInterfaceType">
        ///     The open generic interface type to scan for
        ///     (e.g. typeof(IBaseHandler&lt;,&gt;)).
        /// </param>
        /// <param name="assembly">
        ///     The assembly to scan. If null, the calling assembly is used.
        /// </param>
        /// <param name="lifetime">
        ///     The service lifetime of the registered implementations.
        /// </param>
        /// <returns>The current <see cref="IServiceCollection"/> instance.</returns>
        IServiceCollection AddImplementationByInterfaceFromScanAssembly(Type scanInterfaceType, Assembly? assembly = null, 
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            assembly ??= Assembly.GetCallingAssembly();
            var servicesScanType = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false })
                .SelectMany(t => t.GetInterfaces() .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == scanInterfaceType)
                    .Select(i => new
                    {
                        InterfaceType = i,
                        ImplementationType = t
                    }));
            foreach (var serviceType in servicesScanType)
            {
                var descriptor = new ServiceDescriptor(
                    serviceType.InterfaceType,
                    serviceType.ImplementationType,
                    lifetime);
                serviceCollection.Add(descriptor);
            }
            return serviceCollection;
        }
    }
}
