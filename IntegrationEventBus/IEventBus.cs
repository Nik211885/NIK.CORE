namespace NIK.CORE.DOMAIN.IntegrationEventBus;

/// <summary>
///     Defines an event bus for publishing integration events
///     to communicate across different bounded contexts.
/// </summary>
/// <typeparam name="TYpeMessage">
///     Type of the integration event.
/// </typeparam>
public interface IEventBus<in TYpeMessage>
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
    Task Publish(TYpeMessage @event, CancellationToken cancellation = default);
}
