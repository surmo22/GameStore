using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.GameServices.GameSetupStrategies;

public class CreationDateSetupStep(IDateTimeProvider timeProvider) : ISyncGameSetupStep
{
    public void InitializeField(Game game, IGameRequest gameRequest)
    {
        game.CreationDate = timeProvider.UtcNow;
    }
}