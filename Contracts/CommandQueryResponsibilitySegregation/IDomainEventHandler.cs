namespace NIK.CORE.DOMAIN.Contracts.CommandQueryResponsibilitySegregation;

/// <summary>
///     Defines a handler for processing domain events within
///     the same bounded context.
/// </summary>
/// <typeparam name="TEvent">
///     The type of domain event to handle.
/// </typeparam>
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    /// <summary>
    ///     Handles the specified domain event.
    /// </summary>
    /// <param name="event">
    ///     The domain event that has occurred.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    Task Handle(
        TEvent @event,
        CancellationToken cancellationToken = default
    );
}