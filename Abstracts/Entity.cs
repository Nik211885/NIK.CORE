using NIK.CORE.DOMAIN.Contracts.CommandQueryResponsibilitySegregation;

namespace NIK.CORE.DOMAIN.Abstracts;

/// <summary>
///     Base abstract class representing a domain entity.
/// </summary>
/// <typeparam name="TKey">
///     Type of the entity identifier.
/// </typeparam>
public abstract class Entity<TKey>
{
    /// <summary>
    ///     Stores domain events raised by the entity.
    /// </summary>
    private List<IDomainEvent>? _events;

    /// <summary>
    ///     Gets the unique identifier of the entity.
    /// </summary>
    public virtual TKey Id { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Entity{TKey}"/> class.
    /// </summary>
    /// <param name="id">
    ///     Unique identifier of the entity.
    /// </param>
    protected Entity(TKey id)
    {
        Id = id;
    }

    /// <summary>
    ///     Raises a domain event and adds it to the entity event collection.
    /// </summary>
    /// <param name="event">
    ///     Domain event to be raised.
    /// </param>
    public void RaiseDomainEvent(IDomainEvent @event)
    {
        _events ??= [];
        _events.Add(@event);
    }

    /// <summary>
    ///     Gets all domain events raised by the entity.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents
        => _events ?? [];

    /// <summary>
    ///     Removes all domain events from the entity.
    /// </summary>
    public void ClearDomainEvent() => _events?.Clear();

    /// <summary>
    ///     Removes a specific domain event from the entity.
    /// </summary>
    /// <param name="event">
    ///     Domain event to remove.
    /// </param>
    public void RemoveDomainEvent(IDomainEvent @event) => _events?.Remove(@event);

    /// <summary>
    ///     Determines whether the specified object is equal to the current entity.
    ///     Two entities are considered equal if they are of the same type and
    ///     have the same non-default identifier.
    /// </summary>
    /// <param name="obj">
    ///     The object to compare with the current entity.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the specified object is equal to the current entity;
    ///     otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType() || obj is not Entity<TKey> other)
        {
            return false;
        }

        // Reference equality check
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (this.GetType() != obj.GetType())
        {
            return false;
        }

        if (EqualityComparer<TKey>.Default.Equals(Id, default) 
            || EqualityComparer<TKey>.Default.Equals(other.Id, default))
        {
            return false;
        }

        return EqualityComparer<TKey>.Default.Equals(other.Id, this.Id);
    }

    /// <summary>
    ///     Determines whether two entities are equal.
    ///     Two entities are equal if they have the same identifier.
    /// </summary>
    /// <param name="entity">
    ///     The first entity to compare.
    /// </param>
    /// <param name="entityCompare">
    ///     The second entity to compare.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the entities are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(Entity<TKey> entity, Entity<TKey> entityCompare)
    {
        if (Equals(entity, null))
        {
            return Equals(entityCompare, null);
        }

        return entity.Equals(entityCompare);
    }

    /// <summary>
    ///     Determines whether two entities are not equal.
    /// </summary>
    /// <param name="entity">
    ///     The first entity to compare.
    /// </param>
    /// <param name="entityCompare">
    ///     The second entity to compare.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the entities are not equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(Entity<TKey> entity, Entity<TKey> entityCompare)
        => !(entity == entityCompare);

    /// <summary>
    ///     Returns a hash code for the entity based on its identifier.
    /// </summary>
    /// <returns>
    ///     A hash code for the current entity.
    /// </returns>
    public override int GetHashCode()
        => HashCode.Combine(Id);
}
