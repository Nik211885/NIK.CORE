using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NIK.CORE.DOMAIN.Outbox;

/// <summary>
/// Represents an outbox message used to ensure reliable
/// publishing of integration events.
/// </summary>
/// <remarks>
/// This entity implements the Outbox pattern by persisting
/// integration events in the same transaction as domain changes.
/// <para>
/// Messages stored here are later published to a message broker
/// by a background processor, ensuring no events are lost even
/// in the presence of failures.
/// </para>
/// </remarks>
[Table("OutboxMessages")]
public class OutboxMessage
{
    /// <summary>
    /// Gets the unique identifier of the outbox message.
    /// </summary>
    /// <remarks>
    /// This identifier is also used as the message identifier
    /// when publishing to the message broker to support idempotency.
    /// </remarks>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the integration event type.
    /// </summary>
    /// <remarks>
    /// Typically contains the fully qualified CLR type name
    /// of the integration event.
    /// </remarks>
    [Required]
    [MaxLength(255)]
    public string MessageType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serialized message payload.
    /// </summary>
    /// <remarks>
    /// The content is usually serialized as JSON and represents
    /// the integration event data.
    /// </remarks>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets the UTC timestamp when the message was created.
    /// </summary>
    /// <remarks>
    /// Used for ordering messages during processing.
    /// </remarks>
    public DateTime OccurredOnUtc { get; set; }

    /// <summary>
    /// Gets the UTC timestamp when the message was successfully processed.
    /// </summary>
    /// <remarks>
    /// This value is set after the message has been published
    /// to the message broker.
    /// </remarks>
    public DateTime? ProcessedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the current processing status of the message.
    /// </summary>
    public OutboxStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the last error encountered while processing the message.
    /// </summary>
    /// <remarks>
    /// This value is populated when publishing fails and is intended
    /// for diagnostics and troubleshooting purposes.
    /// </remarks>
    public string? Error { get; set; }
    /// <summary>
    ///    Time to created outbox message
    /// </summary>
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxMessage"/> class.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the message.
    /// </param>
    /// <param name="occurredOn">
    /// The UTC time when the integration event occurred.
    /// </param>
    /// <param name="type">
    /// The type of the integration event.
    /// </param>
    /// <param name="data">
    /// The serialized payload of the integration event.
    /// </param>
    public OutboxMessage(Guid id, DateTime occurredOn, string type, string data)
    {
        Id = id;
        OccurredOnUtc = occurredOn;
        MessageType = type;
        Content = data;
        Status = OutboxStatus.Pending;
    }

    // Required by EF Core
    private OutboxMessage()
    {
    }
}

/// <summary>
/// Represents the processing status of an outbox message.
/// </summary>
/// <remarks>
/// The status reflects the final outcome of the message lifecycle
/// and is intentionally kept minimal to reduce complexity.
/// </remarks>
public enum OutboxStatus
{
    /// <summary>
    /// Message has been created but has not yet been published.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Message has been successfully published to the message broker.
    /// </summary>
    Published = 1,

    /// <summary>
    /// Message has permanently failed and will no longer be retried.
    /// </summary>
    Dead = 2
}
