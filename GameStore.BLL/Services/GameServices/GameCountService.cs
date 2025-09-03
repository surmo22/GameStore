using GameStore.BLL.Interfaces.GameServices;

namespace GameStore.BLL.Services.GameServices;

public class GameCountService : IGameCountService
{
    private int _count;

    public GameCountService(int number)
    {
        if (number < 0)
        {
            throw new InvalidOperationException("Number is less than 0");
        }

        _count = number;
    }

    public int GetTotalGamesCount()
    {
        return _count;
    }

    public int IncrementTotalGamesCount()
    {
        return Interlocked.Increment(ref _count);
    }

    public int DecrementTotalGamesCount()
    {
        return Interlocked.Decrement(ref _count);
    }
}