using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.GameServices.GameSetupStrategies;

public class GameKeySetupStep : ISyncGameSetupStep
{
    public void InitializeField(Game game, IGameRequest gameRequest)
    {
        if (string.IsNullOrEmpty(game.Key))
        {
            game.Key = game.Name;
        }
    }
}