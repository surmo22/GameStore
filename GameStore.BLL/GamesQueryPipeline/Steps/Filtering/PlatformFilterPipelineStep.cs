using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.GamesQueryPipeline.Steps.Filtering;

public class PlatformFilterPipelineStep(GameFilterRequest filterRequest) : IGamePipelineStep
{
    public IQueryable<Game> Process(IQueryable<Game> query)
    {
        if (filterRequest.Platforms.Count != 0)
        {
            query = query.Where(g => g.Platforms.Any(p => filterRequest.Platforms.Contains(p.Id)));
        }

        return query;
    }

    public IQueryable<MongoGame> Process(IQueryable<MongoGame> query)
    {
        // mongo doesn't have platforms, so if user selects platform we remove mongo games from result set
        if (filterRequest.Platforms.Count != 0)
        {
            query = query.Where(game => false);
        }

        return query;
    }
}