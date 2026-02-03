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
    Task<int> SaveChangeAsync(CancellationToken cancellationToken);

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
}
