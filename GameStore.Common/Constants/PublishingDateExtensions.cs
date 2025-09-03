namespace GameStore.Common.Constants;

public static class PublishingDateExtensions
{
    private static readonly Dictionary<PublishingDateFilter, TimeSpan> _publishingDates = new()
    {
        { PublishingDateFilter.LastWeek, TimeSpan.FromDays(7) },
        { PublishingDateFilter.LastMonth, TimeSpan.FromDays(30) },
        { PublishingDateFilter.LastYear, TimeSpan.FromDays(365) },
        { PublishingDateFilter.TwoYears, TimeSpan.FromDays(365 * 2) },
        { PublishingDateFilter.ThreeYears, TimeSpan.FromDays(365 * 3) }
    };

    public static TimeSpan GetPublishingDateFilter(this PublishingDateFilter duration)
    {
        if (_publishingDates.TryGetValue(duration, out var timeSpan))
        {
            return timeSpan;
        }
        
        throw new ArgumentException($"Invalid duration string: {duration}");
    }
}