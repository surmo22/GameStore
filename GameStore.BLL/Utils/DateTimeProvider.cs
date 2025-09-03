using GameStore.BLL.Interfaces;

namespace GameStore.BLL.Utils;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}