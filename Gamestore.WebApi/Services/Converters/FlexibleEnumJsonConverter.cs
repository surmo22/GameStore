using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using GameStore.Common.Utils;

namespace GameStore.WebApi.Services.Converters;

/// <summary>
/// A custom JSON converter that provides flexible serialization and deserialization
/// for enumerations. This converter supports parsing enum values from diverse formats,
/// including case-insensitive strings, and serializes enums with enhanced formatting
/// by inserting spaces before capitalized letters for better readability.
/// </summary>
/// <typeparam name="T">
/// The enum type that the converter will handle. Must be a struct and Enum.
/// </typeparam>
public partial class FlexibleEnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
    /// <summary>
    /// Reads and converts the JSON data to the specified enumeration type during deserialization.
    /// The method attempts to parse the enum value from a string, allowing for flexible input formats.
    /// </summary>
    /// <param name="reader">
    /// The reader object used to parse JSON data.
    /// </param>
    /// <param name="typeToConvert">
    /// The type of the enumeration to which the JSON data should be converted.
    /// </param>
    /// <param name="options">
    /// The serializer options that provide information about the deserialization process.
    /// </param>
    /// <returns>
    /// Returns the deserialized value of the specified enumeration type.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the JSON value is null, empty, or invalid for the target enum type.
    /// </exception>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var rawValue = reader.GetString();
        return EnumHelper.ParseEnum<T>(rawValue!);
    }

    /// <summary>
    /// Writes the specified enum value to the JSON output by serializing it
    /// as a string with spaces inserted before capital letters for improved readability.
    /// </summary>
    /// <param name="writer">
    /// The <see cref="Utf8JsonWriter"/> instance to write the serialized data to.
    /// </param>
    /// <param name="value">
    /// The enum value to be serialized.
    /// </param>
    /// <param name="options">
    /// The <see cref="JsonSerializerOptions"/> that provide options for serialization.
    /// </param>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var spaced = MyRegex().Replace(value.ToString(), " $1");
        writer.WriteStringValue(spaced);
    }

    /// <summary>
    /// Returns a regex instance that matches uppercase letters which are not at the start of a word.
    /// This is used to add spaces before uppercase letters to enhance the readability of enum values when serialized.
    /// </summary>
    /// <returns>
    /// A regex instance that matches pattern "(\\B[A-Z])".
    /// </returns>
    [GeneratedRegex("(\\B[A-Z])")]
    private static partial Regex MyRegex();
}