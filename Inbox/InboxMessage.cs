using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NIK.CORE.DOMAIN.Inbox;

/// <summary>
/// Represents an incoming integration message stored using the Inbox Pattern.
/// </summary>
/// <remarks>
/// The Inbox Pattern is used to ensure idempotent and reliable processing of
/// incoming messages from external systems or message brokers.
/// Each message is persisted before being processed to avoid message loss
/// and duplicate handling.
/// </remarks>
[Table("InboxMessages")]
public class InboxMessage
{
    /// <summary>
    /// Gets the unique identifier of the inbox message.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the fully qualified CLR type name of the message payload.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string MessageType { get; set; } = string.Empty;

    /// <summary>
    /// Gets the serialized content of the message payload.
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets the UTC timestamp when the message was received.
    /// </summary>
    [Required]
    public DateTime ReceivedOnUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the UTC timestamp when the message was successfully processed.
    /// </summary>
    public DateTime? ProcessedOnUtc { get; set; }

    /// <summary>
    /// Gets the error message if processing failed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets the current processing status of the inbox message.
    /// </summary>
    public InboxStatus Status { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InboxMessage"/> class.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the message.
    /// </param>
    /// <param name="messageType">
    /// The fully qualified CLR type name of the message payload.
    /// </param>
    /// <param name="content">
    /// The serialized message payload.
    /// </param>
    public InboxMessage(Guid id, string messageType, string content)
    {
        Id = id;
        MessageType = messageType;
        Content = content;
        Status = InboxStatus.New;
        ReceivedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InboxMessage"/> class
    /// for Entity Framework Core.
    /// </summary>
    private InboxMessage()
    {
    }
}

/// <summary>
/// Defines the processing states of an inbox message.
/// </summary>
public enum InboxStatus
{
    /// <summary>
    /// The message has been received but not yet processed.
    /// </summary>
    New = 0,

    /// <summary>
    /// The message is currently being processed.
    /// </summary>
    Processing = 1,

    /// <summary>
    /// The message has been successfully processed.
    /// </summary>
    Processed = 2,

    /// <summary>
    /// Processing of the message failed.
    /// </summary>
    Failed = 3
}

