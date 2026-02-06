namespace NIK.CORE.DOMAIN.CommandQueryResponsibilitySegregation.Contracts;

public interface IMediator
{
    /// <summary>
    ///     Sends a command that does not return a response.
    /// </summary>
    /// <param name="command">
    ///     The command to send.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask"/> representing the asynchronous operation.
    /// </returns>
    ValueTask Send(ICommand command, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sends a command that returns a response.
    /// </summary>
    /// <param name="command">
    ///     The command to send.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    /// <typeparam name="TResponse">
    ///     Type of the response returned by the command.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="ValueTask{TResponse}"/> representing the asynchronous operation.
    /// </returns>
    ValueTask<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sends a query and returns a response without modifying system state.
    /// </summary>
    /// <param name="query">
    ///     The query to send.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    /// <typeparam name="TResponse">
    ///     Type of the response returned by the query.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="ValueTask{TResponse}"/> representing the asynchronous operation.
    /// </returns>
    ValueTask<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Publishes the specified domain event to all registered handlers.
    /// </summary>
    /// <param name="domainEvent">
    ///     The domain event that has occurred.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous publish operation.
    /// </returns>
    Task Publish(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="domainEvent"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task Publish<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IDomainEvent;
}
