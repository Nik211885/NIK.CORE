namespace NIK.CORE.DOMAIN.CommandQueryResponsibilitySegregation.Contracts;

/// <summary>
/// Defines a generic handler responsible for processing a request
/// and returning a corresponding response asynchronously.
/// </summary>
/// <typeparam name="TRequest">
/// The type of request being handled.
/// Must implement <see cref="IBase{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of response returned after handling the request.
/// </typeparam>
public interface IBaseHandler<in TRequest, TResponse>
    where TRequest : IBase<TResponse>
{
    /// <summary>
    /// Handles the specified request and produces a response.
    /// This method may perform business logic, validation,
    /// and state modifications as required.
    /// </summary>
    /// <param name="request">
    /// The request object containing all data required to perform the operation.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResponse}"/> representing the asynchronous
    /// operation that returns a response.
    /// </returns>
    ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}
