using System.Text.Json;
using System.Text.Json.Serialization;
using NIK.CORE.DOMAIN.Abstracts;

namespace NIK.CORE.DOMAIN.Helpers;

/// <summary>
/// JsonConverterFactory for all TypedIdValueBase-derived types.
/// This factory dynamically creates a generic TypeIdJsonConverter
/// at runtime based on the concrete TypeId type being serialized/deserialized.
/// </summary>
public class TypeIdJsonConverterFactory : JsonConverterFactory
{
    /// <summary>
    /// Determines whether this factory can create a converter
    /// for the specified type.
    /// </summary>
    /// <param name="typeToConvert">The type to check.</param>
    /// <returns>
    /// True if the type inherits from TypedIdValueBase; otherwise, false.
    /// </returns>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(TypedIdValueBase).IsAssignableFrom(typeToConvert);
    }
    /// <summary>
    /// Creates a JsonConverter for the specified TypedId type.
    /// </summary>
    /// <param name="typeToConvert">The concrete TypedId type.</param>
    /// <param name="options">Serializer options.</param>
    /// <returns>A JsonConverter instance for the given type.</returns>
    /// <exception cref="JsonException">
    /// Thrown if the converter instance cannot be created.
    /// </exception>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(TypeIdJsonConverter<>)
            .MakeGenericType(typeToConvert);
        var instance = Activator.CreateInstance(converterType, options);
        if (instance is null)
        {
            throw new JsonException($"Could not create instance of {typeToConvert}");
        }
        return (JsonConverter)instance;
    }
}
