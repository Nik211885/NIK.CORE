namespace NIK.CORE.DOMAIN.Inbox;
/// <summary>
/// Defines persistence operations for inbox messages used by the Inbox Pattern.
/// </summary>
/// <remarks>
/// The Inbox Pattern ensures reliable and idempotent processing of incoming
/// integration messages by persisting them before handling.
/// This interface abstracts storage concerns and enables different
/// infrastructure implementations.
/// </remarks>
public interface IInboxStore
{
    /// <summary>
    /// Determines whether an inbox message with the specified identifier already exists.
    /// </summary>
    /// <param name="messageId">
    /// The unique identifier of the incoming message.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// <c>true</c> if a message with the given identifier already exists;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is typically used to prevent duplicate message processing
    /// and ensure idempotency.
    /// </remarks>
    Task<bool> ExistsAsync(
        Guid messageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new inbox message to the store.
    /// </summary>
    /// <param name="message">
    /// The inbox message to be added.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous add operation.
    /// </returns>
    Task AddAsync(
        InboxMessage message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an inbox message by its unique identifier.
    /// </summary>
    /// <param name="messageId">
    /// The unique identifier of the inbox message.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The inbox message if found; otherwise, <c>null</c>.
    /// </returns>
    Task<InboxMessage?> GetByIdAsync(
        Guid messageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing inbox message in the store.
    /// </summary>
    /// <param name="message">
    /// The inbox message with updated state or error information.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous update operation.
    /// </returns>
    Task UpdateAsync(
        InboxMessage message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes inbox messages older than the specified date.
    /// </summary>
    /// <param name="olderThan">
    /// The cutoff date; messages received before this date are eligible for deletion.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The number of inbox messages that were deleted.
    /// </returns>
    /// <remarks>
    /// This method is typically used by a scheduled cleanup job to
    /// prevent unbounded growth of the inbox table.
    /// </remarks>
    Task<int> DeleteOldMessagesAsync(
        DateTime olderThan,
        CancellationToken cancellationToken = default);
}
