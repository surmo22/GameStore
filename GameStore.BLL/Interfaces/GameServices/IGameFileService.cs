using GameStore.BLL.DTOs.Games;

namespace GameStore.BLL.Interfaces.GameServices;

public interface IGameFileService
{
    Task<string> CreateGameFileAsync(GameDto game);
}
