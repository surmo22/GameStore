using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.GamesQueryPipeline.Interfaces;

public interface IGamePipeline
{
    public IGamePipeline AddStep(IGamePipelineStep step);
    
    IGamePipeline WithPagination(IGamePipelinePaginationStep paginationStep);
    
    IGamePipeline WithSorting(IGamePipelineOrderingStep sortingStep);
    
    IEnumerable<Game> ResortAndRepaginate(IEnumerable<Game> games);
    
    IQueryable<Game> ApplyFiltersAndSorting(IQueryable<Game> query);
    
    IQueryable<MongoGame> ApplyFiltersAndSorting(IQueryable<MongoGame> query);
    
    IQueryable<Game> ApplyPagination(IQueryable<Game> query);
    
    IQueryable<MongoGame> ApplyPagination(IQueryable<MongoGame> query);
}