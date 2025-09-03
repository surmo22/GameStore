using GameStore.BLL.DTOs.Games;

namespace GameStore.BLL.Interfaces.GameServices;

public interface IFilePathService
{
    string GetFilePath(GameDto game);
}