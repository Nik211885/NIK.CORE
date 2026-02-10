using Microsoft.Extensions.Logging;

namespace NIK.CORE.DOMAIN.Outbox
{
    /// <summary>
    /// Performs cleanup of processed outbox messages older than a specified retention period.
    /// </summary>
    /// <remarks>
    /// This job helps prevent unbounded growth of the Outbox table by periodically
    /// deleting messages that have already been successfully processed.
    /// </remarks>
    public sealed class OutboxCleanupJob
    {
        private readonly IOutboxStore _outboxStore;
        private readonly ILogger<OutboxCleanupJob> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboxCleanupJob"/> class.
        /// </summary>
        /// <param name="outboxStore">
        /// The outbox store used to delete processed outbox messages.
        /// </param>
        /// <param name="logger">
        /// The logger instance used for logging job execution details and errors.
        /// </param>
        public OutboxCleanupJob( IOutboxStore outboxStore, ILogger<OutboxCleanupJob> logger)
        {
            _outboxStore = outboxStore;
            _logger = logger;
        }

        /// <summary>
        /// Executes the outbox cleanup job.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <param name="daysRetained">
        /// The number of days processed outbox messages should be retained.
        /// Messages older than this value will be deleted.
        /// Default value is 7 days.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous cleanup operation.
        /// </returns>
        /// <remarks>
        /// Only outbox messages that have already been processed are eligible for deletion.
        /// Any errors encountered during cleanup are logged but not rethrown.
        /// </remarks>
        public async Task CleanJobAsync( CancellationToken cancellationToken, int daysRetained = 7)
        {
            _logger.LogInformation("Starting Outbox cleanup job...");
            var thresholdDate = DateTime.UtcNow.AddDays(-daysRetained);
            try
            {
                int deletedRows = await _outboxStore
                    .DeleteProcessedMessagesAsync(thresholdDate, cancellationToken);
                _logger.LogInformation("Successfully cleaned up {Count} processed outbox messages older than {Date}.",
                    deletedRows, thresholdDate);
            }
            catch (Exception ex)
            {
                _logger.LogError( ex, "An error occurred while cleaning up the Outbox table.");
            }
        }
    }
}
