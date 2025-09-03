namespace GameStore.Common.Utils;

/// <summary>
/// Provides utility methods for working with enumeration types.
/// </summary>
public static class EnumHelper
{
    /// <summary>
    /// Parses a string representation of an enumeration value into a strongly-typed enum value.
    /// </summary>
    /// <typeparam name="T">The enum type to parse. Must be a <see cref="System.Enum"/>.</typeparam>
    /// <param name="value">The string representation of the enum value to parse. Cannot be null or empty.</param>
    /// <returns>The parsed enum value of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided value is null, empty, or does not match any valid enumeration name in the specified enum type.
    /// </exception>
    public static T ParseEnum<T>(string? value) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or empty", nameof(value));
        }

        var normalized = value.Replace(" ", "", StringComparison.OrdinalIgnoreCase);

        if (Enum.GetNames(typeof(T)).Length == 0)
        {
            throw new ArgumentException($"Invalid value '{value}' for enum type {typeof(T).Name}");
        }

        if (Enum.TryParse(normalized, out T result))
        {
            return result;
        }
        
        throw new ArgumentException($"Invalid value '{value}' for enum type {typeof(T).Name}");
    }
}
