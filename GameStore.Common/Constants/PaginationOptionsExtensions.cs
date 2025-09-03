namespace GameStore.Common.Constants;

public static class PaginationOptionsExtensions
{
    private static readonly Dictionary<PaginationOptions, string> paginationOptions = new()
    {
        { PaginationOptions.All, "All" },
        { PaginationOptions.OneHundred, "100" },
        { PaginationOptions.Fifty, "50" },
        { PaginationOptions.Twenty, "20" },
        { PaginationOptions.Ten, "10" }
    };

    public static PaginationOptions GetPaginatingOptions(this string paginationOption)
    {
        try
        {
            var match = paginationOptions.First(entry =>
                string.Equals(entry.Value, paginationOption, StringComparison.OrdinalIgnoreCase));
            return match.Key;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Invalid pagination option: {paginationOption}", ex);
        }
    }
    
    public static string GetDescription(this PaginationOptions paginationOption)
    {
        if (paginationOptions.TryGetValue(paginationOption, out var description))
        {
            return description;
        }
        
        throw new ArgumentOutOfRangeException(nameof(paginationOption), $"Unknown paginating option: {paginationOption}");
    }
}