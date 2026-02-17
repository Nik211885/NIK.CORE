using Microsoft.Extensions.DependencyInjection;
using NIK.CORE.DOMAIN.Attributes;

namespace NIK.CORE.DOMAIN.Contracts;

/// <summary>
///     Represents a unit of work.
///     A unit of work coordinates transactional operations
///     and ensures that multiple changes are committed to the database
///     as a single atomic operation.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    ///     Begins a new database transaction.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken);
    /// <summary>
    ///     Persists all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    ///     The number of state entries written to the database.
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    /// <summary>
    ///     Commits the current transaction.
    ///     All changes will be permanently applied to the database.
    ///     If an error occurs, the transaction should be rolled back.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    /// <summary>
    ///     Rolls back the current transaction,
    ///     discarding all changes made during the transaction.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Gets the identifier of the currently active transaction, if any.
    /// </summary>
    /// <remarks>
    /// Returns <c>null</c> when no transaction is currently in progress.
    /// This value can be used for logging, diagnostics, or transaction correlation.
    /// </remarks>
    Guid CurrentTransactionId { get; }
    /// <summary>
    /// Executes the specified asynchronous action using a resilient execution strategy.
    /// </summary>
    /// <remarks>
    /// This method wraps the provided action with an execution strategy that may
    /// automatically handle transient failures (such as retries) depending on
    /// the underlying implementation (e.g., database retry policies).
    /// </remarks>
    /// <param name="action">
    /// The asynchronous operation to be executed within the execution strategy.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous execution of the provided action.
    /// </returns>
    Task ExecutionStrategyAsync(Func<Task> action);
}
