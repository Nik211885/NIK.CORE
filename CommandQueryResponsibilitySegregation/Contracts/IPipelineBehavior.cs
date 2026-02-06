namespace NIK.CORE.DOMAIN.CommandQueryResponsibilitySegregation.Contracts;
/// <summary>
///     Represents a delegate that executes the next request handler
///     in the pipeline.
/// </summary>
/// <typeparam name="TResponse">
///     Type of the response returned by the handler.
/// </typeparam>
public delegate ValueTask<TResponse> RequestHandlerDelegate<TResponse>();
/// <summary>
///     Defines a pipeline behavior that allows executing logic
///     before and/or after the request handler is invoked.
/// </summary>
/// <typeparam name="TRequest">
///     Type of the request being handled.
/// </typeparam>
/// <typeparam name="TResponse">
///     Type of the response returned by the handler.
/// </typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IBase<TResponse>
{
    /// <summary>
    ///     Handles the specified request and controls the execution
    ///     of the next handler in the pipeline.
    /// </summary>
    /// <param name="request">
    ///     The incoming request.
    /// </param>
    /// <param name="next">
    ///     A delegate that invokes the next handler in the pipeline.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation
    ///     and contains the handler response.
    /// </returns>
    ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default);
}
