using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.CommandQueryResponsibilitySegregation.Contracts;
namespace NIK.CORE.DOMAIN.CommandQueryResponsibilitySegregation;

/// <summary>
///     Default implementation of <see cref="IMediator"/>.
///     <para>
///     Responsible for dispatching commands, queries, and domain events
///     to their corresponding handlers using the Mediator pattern.
///     </para>
///     <para>
///     Supports pipeline behaviors for cross-cutting concerns
///     such as validation, logging, transactions, etc.
///     </para>
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    // Cache for reflection methods (metadata only - safe to cache statically)
    private static readonly ConcurrentDictionary<Type, MethodInfo> MethodCache = new();
    // Cache for closed generic types (metadata only - safe to cache statically)
    private static readonly ConcurrentDictionary<string, Type> ClosedTypeCache = new();
    // Cache for delegate factories (NOT instances - only the MethodInfo needed to create delegates)
    private static readonly ConcurrentDictionary<Type, MethodInfo> HandlerMethodCache = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> PipelineMethodCache = new();
    /// <summary>
    ///     Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">
    ///     Service provider used to resolve handlers and pipeline behaviors.
    /// </param>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///     Sends a command that does not return a response.
    /// </summary>
    public async ValueTask Send(
        ICommand command,
        CancellationToken cancellationToken = default)
        => await SendCore(command, cancellationToken);

    /// <summary>
    ///     Sends a command that returns a response.
    /// </summary>
    public ValueTask<TResponse> Send<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default)
        => SendCore(command, cancellationToken);

    /// <summary>
    ///     Sends a query and returns a response.
    /// </summary>
    public ValueTask<TResponse> Send<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
        => SendCore(query, cancellationToken);

    /// <summary>
    /// Core method responsible for executing a request
    /// through its handler and pipeline behaviors.
    /// </summary>
    ///  <typeparam name="TRequest">The request type.
    /// </typeparam>
    /// <typeparam name="TResponse">The response type.
    /// </typeparam>
    /// <param name="request">The request instance.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.
    /// </param>
    /// <returns>The response produced by the handler.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when no handler is registered for the request type.
    /// </exception>
    public ValueTask<TResponse> Send<TRequest, TResponse>(TRequest request,
        CancellationToken cancellationToken = default) where TRequest : IBase<TResponse>
    {
        var handler = _serviceProvider.GetService<IBaseHandler<TRequest, TResponse>>();
        ArgumentNullException.ThrowIfNull(handler);
        var pipelineBehaviors = _serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>().Reverse();
        RequestHandlerDelegate<TResponse> next = () => handler.Handle(request, cancellationToken);
        foreach (var behavior in pipelineBehaviors)
        {
            var currentNext = next;
            next = () => behavior.Handle(request, currentNext, cancellationToken);
        }
        return next();
    }

    /// <summary>
    ///     Publishes a domain event to all registered handlers (runtime type).
    /// </summary>
    public async Task Publish(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var eventType = domainEvent.GetType();
        var handlerType = GetOrCreateClosedType(
            typeof(IDomainEventHandler<>),
            eventType);
        var handlers = _serviceProvider.GetServices(handlerType);
        foreach (var handler in handlers)
        {
            ArgumentNullException.ThrowIfNull(handler);
            var method = GetCachedMethod(handlerType, nameof(IDomainEventHandler<IDomainEvent>.Handle));
            ArgumentNullException.ThrowIfNull(method);
            var result = method.Invoke(handler, [domainEvent, cancellationToken]);
            if (result is Task task)
            {
                await task;
            }
        }
    }
    /// <summary>
    ///     Publishes a domain event using generic resolution.
    /// </summary>
    public async Task Publish<TEvent>(
        TEvent domainEvent,
        CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        var handlers = _serviceProvider.GetServices<IDomainEventHandler<TEvent>>();
        foreach (var handler in handlers)
        {
            await handler.Handle(domainEvent, cancellationToken);
        }
    }
    /// <summary>
    ///     Core method that resolves handler and executes pipeline behaviors.
    /// </summary>
    private ValueTask<TResponse> SendCore<TResponse>(
        IBase<TResponse> request,
        CancellationToken cancellationToken)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);
        var handler = ResolveService(typeof(IBaseHandler<,>), requestType, responseType);
        var handlerDelegate = GetOrCreateHandlerDelegate<TResponse>(handler, requestType);
        RequestHandlerDelegate<TResponse> next = () => handlerDelegate(request, cancellationToken);
        var pipelineType = GetOrCreateClosedType(
            typeof(IPipelineBehavior<,>),
            requestType,
            responseType);
        var pipelines = _serviceProvider.GetServices(pipelineType).Reverse();
        foreach (var pipeline in pipelines)
        {
            ArgumentNullException.ThrowIfNull(pipeline);
            var currentNext = next;
            var pipelineDelegate = GetOrCreatePipelineDelegate<TResponse>(pipeline, pipelineType);
            next = () => pipelineDelegate(request, currentNext, cancellationToken);
        }
        return next();
    }
    /// <summary>
    ///     Resolves a service from the container using a runtime generic type.
    /// </summary>
    private object ResolveService(Type openGeneric, params Type[] typeArguments)
    {
        var closedType = GetOrCreateClosedType(openGeneric, typeArguments);
        var service = _serviceProvider.GetService(closedType);
        ArgumentNullException.ThrowIfNull(service);
        return service;
    }

    /// <summary>
    ///     Gets or creates a cached closed generic type.
    /// </summary>
    private static Type GetOrCreateClosedType(Type openGeneric, params Type[] typeArguments)
    {
        var cacheKey = $"{openGeneric.FullName}[{string.Join(",", typeArguments.Select(t => t.FullName))}]";
        return ClosedTypeCache.GetOrAdd(cacheKey, _ => openGeneric.MakeGenericType(typeArguments));
    }
    /// <summary>
    ///     Gets cached method info for a type.
    /// </summary>
    private static MethodInfo GetCachedMethod(Type type, string methodName)
    {
        var cacheKey = type;
        return MethodCache.GetOrAdd(cacheKey, _ =>
        {
            var method = type.GetMethod(methodName);
            ArgumentNullException.ThrowIfNull(method);
            return method;
        });
    }

    /// <summary>
    ///     Gets or creates a cached handler delegate using reflection.
    ///     Only caches the MethodInfo, creates a new delegate for each handler instance.
    /// </summary>
    private static Func<IBase<TResponse>, CancellationToken, ValueTask<TResponse>>
        GetOrCreateHandlerDelegate<TResponse>(object handler, Type requestType)
    {
        var handlerType = GetOrCreateClosedType(
            typeof(IBaseHandler<,>),
            requestType,
            typeof(TResponse));
        var method = HandlerMethodCache.GetOrAdd(handlerType, type =>
        {
            var m = type.GetMethod(nameof(IBaseHandler<IBase<TResponse>, TResponse>.Handle));
            ArgumentNullException.ThrowIfNull(m);
            return m;
        });
        return CreateDelegate<Func<IBase<TResponse>, CancellationToken, ValueTask<TResponse>>>(handler, method);
    }

    /// <summary>
    ///     Gets or creates a cached pipeline behavior delegate.
    ///     Only caches the MethodInfo, creates a new delegate for each pipeline instance.
    /// </summary>
    private static Func<IBase<TResponse>, RequestHandlerDelegate<TResponse>, CancellationToken, ValueTask<TResponse>>
        GetOrCreatePipelineDelegate<TResponse>(object pipeline, Type pipelineType)
    { 
        var method = PipelineMethodCache.GetOrAdd(pipelineType, type =>
        {
            var m = type.GetMethod(nameof(IPipelineBehavior<IBase<TResponse>, TResponse>.Handle));
            ArgumentNullException.ThrowIfNull(m);
            return m;
        });
        return CreateDelegate<Func<IBase<TResponse>, RequestHandlerDelegate<TResponse>, CancellationToken, ValueTask<TResponse>>>(
            pipeline,
            method);
    }

    /// <summary>
    ///     Creates a delegate from a target object and method info.
    /// </summary>
    private static TDelegate CreateDelegate<TDelegate>(
        object target,
        MethodInfo method)
        where TDelegate : Delegate
    {
        return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), target, method);
    }
}
