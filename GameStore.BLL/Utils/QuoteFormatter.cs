namespace GameStore.BLL.Utils;

public static class QuoteFormatter
{
    /// <summary>
    /// Formats a quote in the following way: "[name] said: "[body]"".
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="body">Body.</param>
    /// <returns>Formatted quote.</returns>
    public static string FormatQuote(string name, string body)
    {
        return name + " said: \"" + body + "\"";
    }
}