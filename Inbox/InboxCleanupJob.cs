using Hangfire;
using Microsoft.Extensions.Logging;

namespace NIK.CORE.DOMAIN.Inbox;
/// <summary>
/// Represents a scheduled background job that cleans up processed inbox messages.
/// </summary>
/// <remarks>
/// This job is intended to be executed by Hangfire and helps control the size
/// of the Inbox table by removing messages that have already been processed
/// and exceed the configured retention period.
/// </remarks>
public sealed class InboxCleanupJob
{
    private readonly IInboxStore _inboxStore;
    private readonly ILogger<InboxCleanupJob> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="InboxCleanupJob"/> class.
    /// </summary>
    /// <param name="inboxStore">
    /// The inbox store used to delete processed inbox messages.
    /// </param>
    /// <param name="logger">
    /// The logger used for diagnostic and error logging.
    /// </param>
    public InboxCleanupJob( IInboxStore inboxStore, ILogger<InboxCleanupJob> logger)
    {
        _inboxStore = inboxStore;
        _logger = logger;
    }

    /// <summary>
    /// Executes the inbox cleanup job.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <param name="daysRetained">
    /// The number of days processed inbox messages should be retained.
    /// Messages older than this value will be deleted.
    /// The default retention period is 30 days.
    /// </param>
    /// <remarks>
    /// This job is configured to:
    /// <list type="bullet">
    /// <item>Run without automatic retries.</item>
    /// <item>Prevent concurrent executions.</item>
    /// <item>Be safely re-runnable.</item>
    /// </list>
    /// </remarks>
    [JobDisplayName("Inbox: Cleanup Processed Messages")]
    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task CleanJobAsync(
        CancellationToken cancellationToken,
        int daysRetained = 30)
    {
        _logger.LogInformation("Starting Inbox cleanup job...");
        var thresholdDate = DateTime.UtcNow.AddDays(-daysRetained);
        try
        {
            int deletedRows = await _inboxStore
                .DeleteOldMessagesAsync(thresholdDate, cancellationToken);
            _logger.LogInformation(
                "Successfully cleaned up {Count} processed inbox messages older than {Date}.",
                deletedRows, thresholdDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while cleaning up the Inbox table.");
            throw;
        }
    }
}
