using GameStore.Common.Exceptions;

namespace GameStore.Common.Constants;

public static class BanDurationExtensions
{
    public static TimeSpan ConvertBanDurationToTimeSpan(this BanDuration duration)
    {
        return duration switch
        {
            BanDuration.OneHour => TimeSpan.FromHours(1),
            BanDuration.OneDay => TimeSpan.FromDays(1),
            BanDuration.OneWeek => TimeSpan.FromDays(7),
            BanDuration.OneMonth => TimeSpan.FromDays(30),
            BanDuration.Permanent => TimeSpan.FromDays(999999),
            _ => throw new InvalidBanDurationException(duration),
        };
    }
}