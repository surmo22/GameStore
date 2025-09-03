using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.GamesQueryPipeline.Steps.Pagination;

public class PaginationPipelineStep(GameFilterRequest filterRequest) : IGamePipelinePaginationStep
{
    public IQueryable<T> Paginate<T>(IQueryable<T> query)
    {
        if (filterRequest.PageCount == "all" || string.IsNullOrEmpty(filterRequest.PageCount))
        {
            return query;
        }
        
        var pageSize = int.Parse(filterRequest.PageCount);
        return query.Take(pageSize * filterRequest.Page);
    }

    public IEnumerable<Game> RepaginateGames(IEnumerable<Game> games)
    {
        if (filterRequest.PageCount == "all" || string.IsNullOrEmpty(filterRequest.PageCount))
        {
            return games;
        }
        
        var pageSize = int.Parse(filterRequest.PageCount);
        return games.Skip(pageSize * filterRequest.Page - pageSize).Take(pageSize);
    }
}