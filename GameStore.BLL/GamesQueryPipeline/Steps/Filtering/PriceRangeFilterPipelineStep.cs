using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.GamesQueryPipeline.Steps.Filtering;

public class PriceRangeFilterPipelineStep(GameFilterRequest filterRequest) : IGamePipelineStep
{
    public IQueryable<Game> Process(IQueryable<Game> query)
    {
        if (filterRequest.MinPrice.HasValue)
        {
            query = query.Where(g => g.Price >= filterRequest.MinPrice.Value);
        }

        if (filterRequest.MaxPrice.HasValue)
        {
            query = query.Where(g => g.Price <= filterRequest.MaxPrice.Value);
        }

        return query;
    }

    public IQueryable<MongoGame> Process(IQueryable<MongoGame> query)
    {
        if (filterRequest.MinPrice.HasValue)
        {
            query = query.Where(g => g.UnitPrice >= filterRequest.MinPrice.Value);
        }

        if (filterRequest.MaxPrice.HasValue)
        {
            query = query.Where(g => g.UnitPrice <= filterRequest.MaxPrice.Value);
        }
        
        return query;
    }
}