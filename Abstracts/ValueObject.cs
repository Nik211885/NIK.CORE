namespace NIK.CORE.DOMAIN.Abstracts;

/// <summary>
///     Base class for value objects.
///     A value object is defined by its properties and fields, not by identity.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    ///     Determines whether two value objects are equal.
    ///     Two value objects are considered equal when all their
    ///     equality components have the same values.
    /// </summary>
    /// <param name="left">
    ///     The first value object to compare.
    /// </param>
    /// <param name="right">
    ///     The second value object to compare.
    /// </param>
    /// <returns>
    ///     <c>true</c> if both value objects are equal; otherwise, <c>false</c>.
    /// </returns>
    private static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
        {
            return false;
        }

        return ReferenceEquals(left, null) || left.Equals(right);
    }

    /// <summary>
    ///     Determines whether two value objects are not equal.
    /// </summary>
    /// <param name="left">
    ///     The first value object to compare.
    /// </param>
    /// <param name="right">
    ///     The second value object to compare.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the value objects are not equal; otherwise, <c>false</c>.
    /// </returns>
    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !EqualOperator(left, right);
    }

    /// <summary>
    ///     Returns the components that are used to determine
    ///     equality for this value object.
    /// </summary>
    /// <returns>
    ///     A sequence of objects that participate in equality comparison.
    /// </returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    ///     Determines whether the specified object is equal to the current value object.
    /// </summary>
    /// <param name="obj">
    ///     The object to compare with the current value object.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the specified object is equal to the current value object;
    ///     otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    ///     Returns a hash code for the current value object
    ///     based on its equality components.
    /// </summary>
    /// <returns>
    ///     A hash code for the current value object.
    /// </returns>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x.GetHashCode())
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    ///     Creates a shallow copy of the current value object.
    /// </summary>
    /// <returns>
    ///     A copy of the current value object.
    /// </returns>
    public ValueObject? Copy()
    {
        return MemberwiseClone() as ValueObject;
    }
}
