namespace NIK.CORE.DOMAIN.Abstracts;

/// <summary>
/// Base class for strongly-typed identifier value objects.
/// </summary>
/// <remarks>
/// Provides type-safe identifiers backed by <see cref="Guid"/> and
/// enforces value-based equality with type comparison.
/// </remarks>
public abstract class TypedIdValueBase : IEquatable<TypedIdValueBase>
{
    /// <summary>
    /// Gets the underlying <see cref="Guid"/> value.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedIdValueBase"/> class.
    /// </summary>
    /// <param name="value">The identifier value.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.
    /// </exception>
    protected TypedIdValueBase(Guid value)
    {
        if (value == Guid.Empty)
            throw new InvalidOperationException("Id value cannot be empty!");
        Value = value;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current identifier.
    /// </summary>
    public override bool Equals(object? obj) => obj is TypedIdValueBase other && Equals(other);
    /// <summary>
    /// Returns the hash code for this identifier.
    /// </summary>
    public override int GetHashCode()=> HashCode.Combine(GetType(), Value);
    /// <summary>
    /// Determines whether this identifier is equal to another identifier.
    /// </summary>
    /// <param name="other">The identifier to compare with.</param>
    /// <returns>
    /// <c>true</c> if both identifiers are of the same type and have the same value;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(TypedIdValueBase? other)
    {
        if (other is null)
            return false;
        return GetType() == other.GetType() && Value == other.Value;
    }
    /// <summary>
    /// Determines whether two identifiers are equal.
    /// </summary>
    public static bool operator ==(TypedIdValueBase? left,TypedIdValueBase? right) => Equals(left, right);
    /// <summary>
    /// Determines whether two identifiers are not equal.
    /// </summary>
    public static bool operator !=(TypedIdValueBase? left,TypedIdValueBase? right)=> !Equals(left, right);
}
