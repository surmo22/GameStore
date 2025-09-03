using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.GameServices.GameSetupStrategies;

public class GamePublisherSetupStep(IPublisherService publisherService) : IAsyncGameSetupStep
{
    public async Task InitializeFieldAsync(Game game, IGameRequest gameRequest, CancellationToken cancellationToken)
    {
        var publisher = await publisherService.GetPublisherByIdNotMappedAsync(gameRequest.PublisherId, cancellationToken)
            ?? throw new KeyNotFoundException($"No Publisher was found with Id {game.PublisherId}");
        game.PublisherId = gameRequest.PublisherId;
        game.Publisher = publisher;
    }
}