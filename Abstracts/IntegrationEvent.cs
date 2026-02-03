namespace NIK.CORE.DOMAIN.Abstracts;

/// <summary>
///     Base class representing an integration event
///     that is published across bounded contexts.
/// </summary>
public abstract class IntegrationEvent
{
    /// <summary>
    ///     Gets the unique identifier of the integration event.
    /// </summary>
    public Guid Id { get; protected set; } = Guid.CreateVersion7();

    /// <summary>
    ///     Gets the timestamp indicating when the integration event was created.
    /// </summary>
    public DateTimeOffset TimeStamp { get; protected set; } = DateTimeOffset.UtcNow;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IntegrationEvent"/> class.
    /// </summary>
    protected IntegrationEvent()
    {
    }
}