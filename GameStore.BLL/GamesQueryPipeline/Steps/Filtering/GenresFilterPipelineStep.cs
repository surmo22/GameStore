using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Common.Utils;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.GamesQueryPipeline.Steps.Filtering;

public class GenresFilterPipelineStep(GameFilterRequest filterRequest) : IGamePipelineStep
{
    public IQueryable<Game> Process(IQueryable<Game> query)
    {
        if (filterRequest.Genres.Count != 0)
        {
            query = query
                .Where(g => g.Genres.Any(genre => filterRequest.Genres.Contains(genre.Id)));
        }

        return query;
    }

    public IQueryable<MongoGame> Process(IQueryable<MongoGame> query)
    {
        if (filterRequest.Genres.Count == 0)
        {
            return query;
        }

        var intIds = filterRequest.Genres.Select(IntToGuidConverter.Convert);
        query = query
            .Where(g => intIds.Contains(g.CategoryId));

        return query;
    }
}