using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.GamesQueryPipeline.Steps.Filtering;

public class PublishingDateFilterPipelineStep(GameFilterRequest filterRequest) : IGamePipelineStep
{
    public IQueryable<Game> Process(IQueryable<Game> query)
    {
        if (!filterRequest.DatePublishing.HasValue)
        {
            return query;
        }

        var timeSpan = filterRequest.DatePublishing.Value.GetPublishingDateFilter();
        var targetDate = DateTime.Today - timeSpan;
        query = query.Where(g => g.CreationDate >= targetDate);

        return query;
    }

    public IQueryable<MongoGame> Process(IQueryable<MongoGame> query)
    {
        // mongo doesn't have creation date, so if user selects filter by date
        // we remove mongo games from result set
        return filterRequest.DatePublishing.HasValue ? query.Where(game => false) : query;
    }
}