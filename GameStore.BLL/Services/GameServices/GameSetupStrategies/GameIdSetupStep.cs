using GameStore.BLL.DTOs;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.GameServices.GameSetupStrategies;

public class GameIdSetupStep(IGuidProvider guidProvider) : ISyncGameSetupStep
{
    public void InitializeField(Game game, IGameRequest gameRequest)
    {
        if (gameRequest is GameCreateRequestDto)
        {
            game.Id = guidProvider.NewGuid();
        }
    }
}