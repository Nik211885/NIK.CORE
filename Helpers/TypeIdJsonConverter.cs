using System.Text.Json;
using System.Text.Json.Serialization;
using NIK.CORE.DOMAIN.Abstracts;

namespace NIK.CORE.DOMAIN.Helpers;

/// <summary>
/// JSON converter for TypedIdValueBase-derived types.
/// Handles serialization and deserialization of strongly-typed IDs
/// backed by a Guid value.
/// </summary>
/// <typeparam name="TId">
/// The concrete TypedId type that inherits from <see cref="TypedIdValueBase"/>.
/// </typeparam>
public class TypeIdJsonConverter<TId> : JsonConverter<TId>
    where TId : TypedIdValueBase
{
    /// <summary>
    /// Reads a JSON string value and converts it into a TypedId instance.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The concrete TypedId type.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>A new instance of the TypedId.</returns>
    /// <exception cref="JsonException">
    /// Thrown when the JSON token is not a string or the TypedId instance
    /// cannot be created.
    /// </exception>
    public override TId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    { 
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string value for TypeId");
        }
        var guid = reader.GetGuid();
        var instance = Activator.CreateInstance(typeToConvert, guid);
        if (instance is null)
        {
            throw new JsonException($"Could not create instance of {typeToConvert}");
        }
        return (TId)instance;
    }

    /// <summary>
    /// Writes the TypedId value as a JSON string.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The TypedId instance to serialize.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, TId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
