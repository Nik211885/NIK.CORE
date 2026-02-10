namespace NIK.CORE.DOMAIN.IntegrationEventBus;

/// <summary>
///     Defines an event bus for publishing integration events
///     to communicate across different bounded contexts.
/// </summary>
public interface IEventBus
{
    /// <summary>
    ///     Publishes an integration event to the event bus.
    /// </summary>
    /// <param name="event">
    ///     The integration event to publish.
    /// </param>
    /// <param name="cancellation">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous publish operation.
    /// </returns>
    Task Publish<TYpeMessage>(TYpeMessage @event, CancellationToken cancellation = default);  
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mess"></param>
    /// <param name="eventType"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task Publish(object mess, Type eventType, CancellationToken cancellation = default);
}
