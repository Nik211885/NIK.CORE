using NIK.CORE.DOMAIN.Abstracts;

namespace NIK.CORE.DOMAIN.IntegrationEventBus;

/// <summary>
///     Defines a handler for processing integration events.
/// </summary>
/// <typeparam name="TYpeMessage">
///     Type of the integration event to handle.
/// </typeparam>
public interface IIntegrationEventHandler<in TYpeMessage>
{
    /// <summary>
    ///     Handles the specified integration event.
    /// </summary>
    /// <param name="event">
    ///     The integration event to handle.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous handling operation.
    /// </returns>
    Task Handle(TYpeMessage @event, CancellationToken cancellationToken = default);
}
