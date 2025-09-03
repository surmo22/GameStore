namespace GameStore.Common.Constants;

public static class SortingOptionsExtensions
{
    private static readonly Dictionary<SortingOptions, string> SortingOptions = new()
    {
        { Constants.SortingOptions.MostPopular, "Most Popular" },
        { Constants.SortingOptions.MostCommented, "Most Commented" },
        { Constants.SortingOptions.PriceAscending, "Price Ascending" },
        { Constants.SortingOptions.PriceDescending, "Price Descending" },
        { Constants.SortingOptions.New, "New" }
    };

    public static SortingOptions GetSortingOptions(this string sortingOption)
    {
        try
        {
            var match = SortingOptions.First(entry =>
                string.Equals(entry.Value, sortingOption, StringComparison.OrdinalIgnoreCase));
            return match.Key;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid sorting option: {sortingOption}", ex);
        }
    }
    
    public static string GetDescription(this SortingOptions sortingOption)
    {
        if (SortingOptions.TryGetValue(sortingOption, out var description))
        {
            return description;
        }
        
        throw new ArgumentOutOfRangeException(nameof(sortingOption), $"Unknown sorting option: {sortingOption}");
    }
}