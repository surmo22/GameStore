using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces.GameServices.ImageServices;
using GameStore.BLL.Interfaces.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.GameServices.GameSetupStrategies;

public class GameImageSetupStep(IGameImageService gameImageService) : IAsyncGameSetupStep
{
    public async Task InitializeFieldAsync(Game game, IGameRequest gameRequest, CancellationToken cancellationToken)
    {
        var base64Image = gameRequest.Image;
        game.ImageUrl = await gameImageService.UploadImageAsync(game.Key, base64Image, cancellationToken);
    }
}