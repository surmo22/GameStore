using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Common.Utils;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.GamesQueryPipeline.Steps.Filtering;

public class PublisherFilterPipelineStep(GameFilterRequest filterRequest) : IGamePipelineStep
{
    public IQueryable<Game> Process(IQueryable<Game> query)
    {
        if (filterRequest.Publisher.HasValue && filterRequest.Publisher.Value != Guid.Empty)
        {
            query = query.Where(g => g.PublisherId == filterRequest.Publisher.Value);
        }
        
        return query;
    }

    public IQueryable<MongoGame> Process(IQueryable<MongoGame> query)
    {
        if (!filterRequest.Publisher.HasValue || filterRequest.Publisher.Value == Guid.Empty)
        {
            return query;
        }

        var intId = IntToGuidConverter.Convert(filterRequest.Publisher.Value);
        query = query.Where(g => g.SupplierId == intId);

        return query;
    }
}