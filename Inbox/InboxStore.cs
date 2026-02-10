using Microsoft.EntityFrameworkCore;
using NIK.CORE.DOMAIN.Abstracts;

namespace NIK.CORE.DOMAIN.Inbox;
/// <summary>
/// Provides Entity Framework Core–based persistence for inbox messages.
/// </summary>
/// <remarks>
/// This class is the default implementation of <see cref="IInboxStore"/> and
/// supports idempotent and reliable message handling as part of the Inbox Pattern.
/// </remarks>
public sealed class InboxStore : IInboxStore
{
    private readonly BaseDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="InboxStore"/> class.
    /// </summary>
    /// <param name="dbContext">
    /// The database context used to access inbox messages.
    /// </param>
    public InboxStore(BaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Determines whether an inbox message with the specified identifier already exists.
    /// </summary>
    /// <param name="messageId">
    /// The unique identifier of the inbox message.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// <c>true</c> if a message with the given identifier exists; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method performs a lightweight existence check without loading
    /// the full entity into memory.
    /// </remarks>
    public async Task<bool> ExistsAsync(
        Guid messageId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.InboxMessages.AsNoTracking()
            .AnyAsync(m => m.Id == messageId, cancellationToken);
    }

    /// <summary>
    /// Adds a new inbox message to the store and persists it to the database.
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
    public async Task AddAsync(
        InboxMessage message,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.InboxMessages.AddAsync(message, cancellationToken);
    }

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
    public async Task<InboxMessage?> GetByIdAsync(
        Guid messageId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.InboxMessages
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);
    }

    /// <summary>
    /// Updates an existing inbox message and persists the changes.
    /// </summary>
    /// <param name="message">
    /// The inbox message with updated status or error information.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous update operation.
    /// </returns>
    public async Task UpdateAsync(
        InboxMessage message,
        CancellationToken cancellationToken = default)
    {
        _dbContext.InboxMessages.Update(message);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes processed inbox messages older than the specified date.
    /// </summary>
    /// <param name="olderThan">
    /// The cutoff date; messages received before this date will be deleted.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The number of inbox messages that were deleted.
    /// </returns>
    /// <remarks>
    /// Uses <c>ExecuteDeleteAsync</c> (EF Core 7+) to perform a bulk delete
    /// operation without loading entities into memory.
    /// Only messages with <see cref="InboxStatus.Processed"/> status
    /// are eligible for deletion.
    /// </remarks>
    public async Task<int> DeleteOldMessagesAsync(
        DateTime olderThan,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.InboxMessages
            .Where(m => m.ReceivedOnUtc < olderThan &&
                        m.Status == InboxStatus.Processed)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
