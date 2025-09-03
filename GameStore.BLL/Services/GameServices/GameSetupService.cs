using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces.GameServices;
using GameStore.BLL.Interfaces.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.GameServices;

public class GameSetupService(IEnumerable<IGameSetupStep> strategies)
    : IGameSetupService
{
    public async Task BuildGameAsync(Game game, IGameRequest gameRequest, CancellationToken cancellationToken)
    {
        foreach (var strategy in strategies)
        {
            if (strategy is IAsyncGameSetupStep asyncGameSetupStrategy)
            {
                await asyncGameSetupStrategy.InitializeFieldAsync(game, gameRequest, cancellationToken);
            }
            else if (strategy is ISyncGameSetupStep syncGameSetupStrategy)
            {
                syncGameSetupStrategy.InitializeField(game, gameRequest);
            }
        }
    }
}