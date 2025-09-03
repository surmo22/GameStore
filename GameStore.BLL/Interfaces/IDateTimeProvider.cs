namespace GameStore.BLL.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}