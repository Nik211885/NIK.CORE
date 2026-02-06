using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NIK.CORE.DOMAIN.Abstracts;
using NIK.CORE.DOMAIN.CommandQueryResponsibilitySegregation.Contracts;

namespace NIK.CORE.DOMAIN.Pipelines.PipesMediator;

/// <summary>
/// MediatR pipeline behavior that manages a database transaction scope
/// for command execution.
/// </summary>
/// <remarks>
/// This pipeline ensures that each command is executed within a single
/// database transaction when no active transaction already exists.
/// <para>
/// If a transaction is already present, the pipeline will reuse it and
/// delegate execution directly to the next behavior or handler.
/// </para>
/// <para>
/// The pipeline leverages EF Core execution strategies to provide resiliency
/// against transient failures while maintaining transactional consistency.
/// </para>
/// </remarks>
/// <typeparam name="TCommand">
/// The command type being processed.
/// </typeparam>
/// <typeparam name="TResponse">
/// The response type returned by the command handler.
/// </typeparam>
public abstract class BaseTransactionScopePipelineBehavior<TCommand, TResponse>
    : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly BaseDbContext _dbContext;
    private readonly ILogger<BaseTransactionScopePipelineBehavior<TCommand, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTransactionScopePipelineBehavior{TCommand,TResponse}"/> class.
    /// </summary>
    /// <param name="dbContext">
    /// The database context used to manage transactions.
    /// </param>
    /// <param name="logger">
    /// The logger used to record transaction lifecycle events.
    /// </param>
    protected BaseTransactionScopePipelineBehavior(
        BaseDbContext dbContext,
        ILogger<BaseTransactionScopePipelineBehavior<TCommand, TResponse>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Handles the execution of a command within a transactional scope.
    /// </summary>
    /// <param name="request">
    /// The command being executed.
    /// </param>
    /// <param name="next">
    /// The delegate representing the next pipeline behavior or handler.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    /// The response produced by the command handler.
    /// </returns>
    /// <remarks>
    /// When no transaction is active, this method:
    /// <list type="number">
    ///   <item>Creates a resilient execution strategy</item>
    ///   <item>Begins a new database transaction</item>
    ///   <item>Executes the command handler</item>
    ///   <item>Commits the transaction upon success</item>
    /// </list>
    /// <para>
    /// Any exception thrown during execution will cause the transaction
    /// to be rolled back by EF Core and propagated to the caller.
    /// </para>
    /// </remarks>
    public async ValueTask<TResponse> Handle(
        TCommand request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var response = default(TResponse);
        var requestTypeName = typeof(TCommand).FullName;

        try
        {
            if (_dbContext.Database.CurrentTransaction is not null)
            {
                _logger.LogInformation(
                    "Transaction is already existing for {requestName}, {request}",
                    requestTypeName,
                    request);
                return await next();
            }
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                Guid? transactionId = _dbContext.Database.CurrentTransaction?.TransactionId;
                _logger.LogInformation(
                    "Beginning new transaction [TransactionId: {transactionId}] for request {requestName}",
                    transactionId,
                    requestTypeName);
                response = await next();
                await _dbContext.Database.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation(
                    "Completed transaction [TransactionId: {transactionId}] for request {requestName}",
                    transactionId,
                    requestTypeName);
            });
            return response!;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling transaction behavior for {commandName}, {command}",
                requestTypeName,
                request);
            throw;
        }
    }
}
