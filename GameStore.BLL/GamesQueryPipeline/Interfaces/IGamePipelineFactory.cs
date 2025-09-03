using GameStore.BLL.DTOs.Games;

namespace GameStore.BLL.GamesQueryPipeline.Interfaces;

public interface IGamePipelineFactory
{
    IGamePipeline Create(GameFilterRequest filterRequest, bool hasPermissionToViewDeletedGames);
}