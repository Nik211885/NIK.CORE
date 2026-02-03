namespace NIK.CORE.DOMAIN.Contracts.CommandQueryResponsibilitySegregation;

/// <summary>
/// Represents a handler responsible for processing a command
/// that performs an action and returns a response.
/// </summary>
/// <typeparam name="TCommand">
/// The type of command being handled.
/// Must implement <see cref="ICommand{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of response returned after the command is executed.
/// </typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IBaseHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;


/// <summary>
/// Represents a handler responsible for processing a command
/// that performs an action without returning a value.
/// </summary>
/// <typeparam name="TCommand">
/// The type of command being handled.
/// Must implement <see cref="ICommand"/>.
/// </typeparam>
public interface ICommandHandler<in TCommand> : IBaseHandler<TCommand, UnitType>
    where TCommand : ICommand;
