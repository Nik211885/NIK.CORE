using System.Text.Json;
using Microsoft.Extensions.Logging;
using NIK.CORE.DOMAIN.CommandQueryResponsibilitySegregation.Contracts;
using NIK.CORE.DOMAIN.Exceptions;
using NIK.CORE.DOMAIN.Validator;

namespace NIK.CORE.DOMAIN.Pipelines.PipesMediator;

/// <summary>
/// Pipeline behavior responsible for validating commands before they reach their handlers.
/// </summary>
/// <remarks>
/// This behavior executes validation logic using <see cref="IValidator{TCommand}"/>.
/// If validation fails, a <see cref="BadRequestException"/> is thrown containing
/// a JSON-formatted error payload grouped by property name.
/// </remarks>
/// <typeparam name="TCommand">The command type being validated.</typeparam>
/// <typeparam name="TResponse">The response type returned by the command handler.</typeparam>
public class CoreValidationPipelineBehavior<TCommand, TResponse> 
    : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly IValidator<TCommand> _validator;
    private readonly ILogger<CoreValidationPipelineBehavior<TCommand, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreValidationPipelineBehavior{TCommand, TResponse}"/> class.
    /// </summary>
    /// <param name="validator">
    /// The validator responsible for validating the incoming command.
    /// </param>
    /// <param name="logger">
    /// The logger used to record validation-related information.
    /// </param>
    public CoreValidationPipelineBehavior(
        IValidator<TCommand> validator, 
        ILogger<CoreValidationPipelineBehavior<TCommand, TResponse>> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Handles the command validation logic before invoking the next pipeline behavior or handler.
    /// </summary>
    /// <param name="request">The incoming command instance.</param>
    /// <param name="next">The delegate representing the next pipeline behavior.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// The response returned by the command handler if validation succeeds.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when validation fails. The exception message contains a JSON string
    /// representing validation errors grouped by property name.
    /// </exception>
    public async ValueTask<TResponse> Handle(
        TCommand request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(request);
        if (validation.IsValid)
        {
            return await next();
        }
        var warningObject = validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => string.Join("; ", g.Select(e => e.ErrorMessage))
            );
        var jsonWarningObject = JsonSerializer.Serialize(warningObject);
        _logger.LogWarning(
            "Validation failed for command {Command}. Errors: {Errors}", 
            typeof(TCommand).Name, 
            jsonWarningObject);
        throw new BadRequestException(jsonWarningObject);
    }
}
