namespace NIK.CORE.DOMAIN.Outbox;

/// <summary>
/// Defines a contract for persisting and managing outbox messages.
/// </summary>
/// <remarks>
/// This interface represents the persistence boundary of the Outbox pattern.
/// <para>
/// It is responsible only for storing, retrieving, updating, and cleaning up
/// outbox messages. The actual publishing logic is handled elsewhere
/// (e.g., background workers or message dispatchers).
/// </para>
/// </remarks>
public interface IOutboxStore
{
    /// <summary>
    /// Adds a new outbox message to the store.
    /// </summary>
    /// <remarks>
    /// This method is typically called within the same database transaction
    /// as the business operation that produced the integration event.
    /// </remarks>
    /// <param name="message">
    /// The outbox message to persist.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    Task AddAsync(
        OutboxMessage message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a batch of unprocessed outbox messages.
    /// </summary>
    /// <remarks>
    /// Returned messages are usually filtered by status (e.g. Pending)
    /// and ordered by occurrence time to ensure deterministic processing.
    /// <para>
    /// The batch size limits the number of messages processed in a single run
    /// to avoid overwhelming the message broker.
    /// </para>
    /// </remarks>
    /// <param name="batchSize">
    /// Maximum number of messages to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A list of unprocessed outbox messages.
    /// </returns>
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(
        int batchSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing outbox message.
    /// </summary>
    /// <remarks>
    /// This method is typically used to update the processing status,
    /// error information, or processed timestamp after a publish attempt.
    /// </remarks>
    /// <param name="message">
    /// The outbox message to update.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    Task UpdateAsync(
        OutboxMessage message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes processed outbox messages older than a specified timestamp.
    /// </summary>
    /// <remarks>
    /// This operation is intended for cleanup purposes only.
    /// <para>
    /// Removing old, successfully processed messages helps keep
    /// the outbox table small and improves query performance.
    /// </para>
    /// </remarks>
    /// <param name="olderThan">
    /// The cutoff UTC timestamp; messages processed before this time
    /// may be deleted.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    Task DeleteProcessedMessagesAsync(
        DateTime olderThan,
        CancellationToken cancellationToken = default);
}
