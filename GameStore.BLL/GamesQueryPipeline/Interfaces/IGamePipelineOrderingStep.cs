using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.GamesQueryPipeline.Interfaces;

public interface IGamePipelineOrderingStep
{
    public IQueryable<Game> OrderGames(IQueryable<Game> games);
    
    public IQueryable<MongoGame> OrderGames(IQueryable<MongoGame> games);
}