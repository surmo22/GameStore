using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.GamesQueryPipeline.Interfaces;

public interface IGamePipelineStep
{
    IQueryable<Game> Process(IQueryable<Game> query);
    
    IQueryable<MongoGame> Process(IQueryable<MongoGame> query);
}