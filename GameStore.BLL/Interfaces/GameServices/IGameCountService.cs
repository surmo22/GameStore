namespace GameStore.BLL.Interfaces.GameServices;

public interface IGameCountService
{
    int GetTotalGamesCount();

    int IncrementTotalGamesCount();

    int DecrementTotalGamesCount();
}