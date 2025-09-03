using System.ComponentModel;
using System.Globalization;

namespace GameStore.Common.Utils;

/// <summary>
/// Provides a flexible enum type converter that allows converting string values into enumeration values
/// by leveraging a case-insensitive comparison and handling whitespace normalization.
/// </summary>
/// <typeparam name="T">
/// The enum type to be converted. Must be a struct and of type <see cref="System.Enum"/>.
/// </typeparam>
/// <remarks>
/// This type converter is useful when string representations of enum values may include variations
/// in capitalization or whitespace. It leverages the <see cref="EnumHelper.ParseEnum{T}(string)"/>
/// method for parsing.
/// </remarks>
/// <example>
/// The converter can be applied to enums via attributes like <see cref="TypeConverterAttribute"/>.
/// </example>
public class FlexibleEnumTypeConverter<T> : TypeConverter where T : struct, Enum
{
    /// <summary>
    /// Determines whether a given type can be converted to the associated enumeration type.
    /// </summary>
    /// <param name="context">An optional format context containing information about the environment this converter is being invoked from. This parameter can be null.</param>
    /// <param name="sourceType">The type of the source data that is being evaluated for conversion.</param>
    /// <returns>
    /// <c>true</c> if the <paramref name="sourceType"/> is a <see cref="string"/>, otherwise <c>false</c>.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string);

    /// <summary>
    /// Converts the given object to the specified enumeration type.
    /// </summary>
    /// <param name="context">An optional format context to provide additional information during conversion.</param>
    /// <param name="culture">The culture to use, or null to use the current culture.</param>
    /// <param name="value">The object to convert, typically a string representing an enumeration value.</param>
    /// <returns>The converted enum value of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the input value is null, empty, or does not match a valid enumeration value for type <typeparamref name="T"/>.</exception>
    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return EnumHelper.ParseEnum<T>(value.ToString());
    }
}