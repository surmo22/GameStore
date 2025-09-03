using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.GameServices.GameSetupStrategies;

public class GamePlatformSetupStep(IPlatformService platformService) : IAsyncGameSetupStep
{
    public async Task InitializeFieldAsync(Game game, IGameRequest gameRequest, CancellationToken cancellationToken)
    {
        if (gameRequest.Platforms is { Count: not 0 })
        {
            game.Platforms = await platformService.GetPlatformsByIdsAsync(gameRequest.Platforms, cancellationToken);
        }
    }
}