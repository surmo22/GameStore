using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.GamesQueryPipeline.Steps.Filtering;

public class NotIncludeDeletedGamesPipelineStep : IGamePipelineStep
{
    public IQueryable<Game> Process(IQueryable<Game> query)
    {
        query = query.Where(g => !g.IsDeleted);
        return query;
    }

    public IQueryable<MongoGame> Process(IQueryable<MongoGame> query)
    {
        // skip, mongo games can be deleted only in SQL
        return query;
    }
}