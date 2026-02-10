using System.Collections.Concurrent;
using System.Text.Json;
using Hangfire;
using Microsoft.Extensions.Logging;
using NIK.CORE.DOMAIN.IntegrationEventBus;

namespace NIK.CORE.DOMAIN.Outbox;
/// <summary>
/// Processes outbox messages and publishes integration events to the event bus.
/// </summary>
/// <remarks>
/// This class is part of the Outbox Pattern implementation, ensuring reliable
/// event publishing by persisting events before dispatching them to the broker.
/// </remarks>
public sealed class OutboxProcessor
{
    private readonly IOutboxStore _outboxStore;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly ConcurrentDictionary<string, Type?> _typeCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxProcessor"/> class.
    /// </summary>
    /// <param name="outboxStore">
    /// The outbox store used to retrieve and update outbox messages.
    /// </param>
    /// <param name="eventBus">
    /// The event bus used to publish integration events.
    /// </param>
    /// <param name="logger">
    /// The logger instance for diagnostic and error logging.
    /// </param>
    public OutboxProcessor( IOutboxStore outboxStore, IEventBus eventBus, ILogger<OutboxProcessor> logger)
    {
        _outboxStore = outboxStore;
        _eventBus = eventBus;
        _logger = logger;
    }

    /// <summary>
    /// Scans the outbox for unprocessed messages and publishes them to the event bus.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <param name="unprocessedMessagesCount">
    /// The maximum number of unprocessed messages to retrieve in one execution.
    /// Default value is 50.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous processing operation.
    /// </returns>
    /// <remarks>
    /// For each outbox message, the payload is deserialized based on its message type,
    /// published to the event bus, and then marked as processed. If an error occurs,
    /// the error message is stored with the outbox record.
    /// </remarks>
    [JobDisplayName("Outbox: ProcessAsync Push message to broker")]
    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task ProcessAsync(
        CancellationToken cancellationToken,
        int unprocessedMessagesCount = 50)
    {
        _logger.LogDebug("Scanning out box to publish events to broker ...");
        var messages = await _outboxStore
            .GetUnprocessedMessagesAsync(unprocessedMessagesCount, cancellationToken);
        if (!messages.Any())
            return;
        foreach (var mess in messages)
        {
            try
            {
                var payloadType = _typeCache.GetOrAdd( mess.MessageType, Type.GetType);
                if (payloadType is null)
                {
                    _logger.LogError("Can't resolve type: {MessageType} for message {MessageId}",
                        mess.MessageType, mess.Id);
                    mess.Status = OutboxStatus.Dead;
                    mess.Error = $"Type {mess.MessageType} not found in assembly.";
                    await _outboxStore.UpdateAsync(mess, cancellationToken);
                    continue;
                }
                var payload = JsonSerializer.Deserialize( mess.Content, payloadType);
                if (payload is not null)
                {
                    await _eventBus.Publish( payload, payloadType, cancellationToken);
                    _logger.LogInformation("Published event successfully. MessageId: {Id}, Type: {MessageType}",
                        mess.Id, mess.MessageType);
                    mess.ProcessedOnUtc = DateTime.UtcNow;
                    mess.Status = OutboxStatus.Published;
                    mess.Error = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError( ex,"Error processing outbox message {Id}", mess.Id); mess.Error = ex.Message;
            }

            await _outboxStore.UpdateAsync(mess, cancellationToken);
        }
    }
}
