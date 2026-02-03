namespace NIK.CORE.DOMAIN.Contracts.CommandQueryResponsibilitySegregation;

/// <summary>
///     Defines a handler for processing queries.
/// </summary>
/// <typeparam name="TQuery">
///     Type of the query to handle.
/// </typeparam>
/// <typeparam name="TResponse">
///     Type of the response returned by the query.
/// </typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IBaseHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;