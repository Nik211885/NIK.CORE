using Microsoft.EntityFrameworkCore;
using NIK.CORE.DOMAIN.Abstracts;

namespace NIK.CORE.DOMAIN.Outbox
{
    /// <summary>
    /// Provides database access for managing outbox messages using Entity Framework Core.
    /// </summary>
    /// <remarks>
    /// This class is the default implementation of <see cref="IOutboxStore"/> and is
    /// responsible for persisting, retrieving, updating, and deleting outbox messages
    /// as part of the Outbox Pattern.
    /// </remarks>
    public class OutboxStore : IOutboxStore
    {
        private readonly BaseDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboxStore"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The database context used to access the OutboxMessages table.
        /// </param>
        public OutboxStore(BaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds a new outbox message to the store.
        /// </summary>
        /// <param name="message">
        /// The outbox message to be added.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous add operation.
        /// </returns>
        /// <remarks>
        /// The message is added to the change tracker but is not persisted
        /// until the surrounding unit of work is committed.
        /// </remarks>
        public async Task AddAsync(
            OutboxMessage message,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
        }

        /// <summary>
        /// Retrieves a batch of unprocessed outbox messages.
        /// </summary>
        /// <param name="batchSize">
        /// The maximum number of unprocessed messages to retrieve.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A list of outbox messages with <see cref="OutboxStatus.Pending"/> status,
        /// ordered by creation time in ascending order.
        /// </returns>
        public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(
            int batchSize,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.OutboxMessages
                .Where(m => m.Status == OutboxStatus.Pending)
                .OrderBy(m => m.CreatedOnUtc)
                .Take(batchSize)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing outbox message and persists the changes.
        /// </summary>
        /// <param name="message">
        /// The outbox message to be updated.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous update operation.
        /// </returns>
        /// <remarks>
        /// This method immediately saves changes to the database.
        /// </remarks>
        public async Task UpdateAsync(
            OutboxMessage message,
            CancellationToken cancellationToken = default)
        {
            _dbContext.OutboxMessages.Update(message);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Deletes processed outbox messages older than the specified date.
        /// </summary>
        /// <param name="olderThan">
        /// The cutoff date; messages created before this date will be deleted.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The number of deleted outbox messages.
        /// </returns>
        /// <remarks>
        /// Only messages that are not in <see cref="OutboxStatus.Pending"/> status
        /// are eligible for deletion.
        /// </remarks>
        public async Task<int> DeleteProcessedMessagesAsync(
            DateTime olderThan,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.OutboxMessages
                .Where(m => m.Status != OutboxStatus.Pending && m.CreatedOnUtc < olderThan)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
