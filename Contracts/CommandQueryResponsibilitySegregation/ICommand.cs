namespace NIK.CORE.DOMAIN.Contracts.CommandQueryResponsibilitySegregation;

/// <summary>
///     Marker interface for commands following the CQRS pattern.
///     A command represents an intention to change system state
///     by creating or updating resources.
/// </summary>
public interface ICommand : ICommand<UnitType>;

/// <summary>
///     Represents a command that returns a response after execution.
/// </summary>
/// <typeparam name="TResponse">
///     Type of the response returned by the command handler.
/// </typeparam>
public interface ICommand<TResponse> : IBase<TResponse>;
