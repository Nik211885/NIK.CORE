namespace NIK.CORE.DOMAIN.CommandQueryResponsibilitySegregation.Contracts;

/// <summary>
///     Represents a query following the CQRS pattern.
///     A query retrieves data without modifying system state
///     and returns a result of type <typeparamref name="TResponse" />.
/// </summary>
/// <typeparam name="TResponse">
///     Type of the data returned by the query.
/// </typeparam>
public interface IQuery<TResponse> : IBase<TResponse>;
