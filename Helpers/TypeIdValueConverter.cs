using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NIK.CORE.DOMAIN.Abstracts;

namespace NIK.CORE.DOMAIN.Helpers;

/// <summary>
/// Generic EF Core value converter for strongly-typed ID classes
/// derived from <see cref="TypedIdValueBase"/>.
/// 
/// This converter maps a TypedId value object to a <see cref="Guid"/>
/// for database storage, and recreates the TypedId when reading from
/// the database.
/// </summary>
/// <typeparam name="TTypedIdValue">
/// The concrete TypedId type (e.g. UserId, OrderId) that inherits from
/// <see cref="TypedIdValueBase"/>.
/// </typeparam>
public class TypedIdValueConverter<TTypedIdValue> : ValueConverter<TTypedIdValue, Guid>
    where TTypedIdValue : TypedIdValueBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypedIdValueConverter{TTypedIdValue}"/>.
    /// </summary>
    /// <param name="mappingHints">
    /// Optional mapping hints used by EF Core when configuring the database column.
    /// </param>
    public TypedIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => Create(value),
            mappingHints)
    {
    }

    /// <summary>
    /// Creates a new instance of the TypedId using its Guid constructor.
    /// </summary>
    /// <param name="id">The Guid value read from the database.</param>
    /// <returns>A new instance of the TypedId.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the TypedId does not define a constructor that accepts a Guid.
    /// </exception>
    private static TTypedIdValue Create(Guid id) =>
        Activator.CreateInstance(typeof(TTypedIdValue), id) as TTypedIdValue
        ?? throw new InvalidOperationException(
            $"Failed to create instance of {typeof(TTypedIdValue).Name}. " +
            $"Ensure it has a constructor that accepts a Guid.");
}
