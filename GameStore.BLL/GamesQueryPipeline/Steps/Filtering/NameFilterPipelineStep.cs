using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.GamesQueryPipeline.Steps.Filtering;

public class NameFilterPipelineStep(GameFilterRequest filterRequest) : IGamePipelineStep
{
    public IQueryable<Game> Process(IQueryable<Game> query)
    {
        if (!string.IsNullOrEmpty(filterRequest.Name))
        {
            query = query.Where(g => g.Name.Contains(filterRequest.Name));
        }
        
        return query;
    }

    public IQueryable<MongoGame> Process(IQueryable<MongoGame> query)
    {
        if (!string.IsNullOrEmpty(filterRequest.Name))
        {
            query = query.Where(g => g.ProductName.Contains(filterRequest.Name));
        }
        
        return query;
    }
}