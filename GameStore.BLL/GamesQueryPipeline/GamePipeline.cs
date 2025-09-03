using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.GamesQueryPipeline;

public class GamePipeline : IGamePipeline
{
    private readonly IList<IGamePipelineStep> _steps = [];
    
    private IGamePipelinePaginationStep _paginationStep;
    
    private IGamePipelineOrderingStep _sortingStep;
    
    public IGamePipeline AddStep(IGamePipelineStep step)
    {
        _steps.Add(step);
        return this;
    }

    public IGamePipeline WithPagination(IGamePipelinePaginationStep paginationStep)
    {
        _paginationStep = paginationStep;
        return this;
    }

    public IGamePipeline WithSorting(IGamePipelineOrderingStep sortingStep)
    {
        _sortingStep = sortingStep;
        return this;
    }

    public IQueryable<Game> ApplyFiltersAndSorting(IQueryable<Game> query)
    {
        query = _steps.Aggregate(query, (current, step) => step.Process(current));
        query = _sortingStep.OrderGames(query);
        return query;
    }

    public IQueryable<MongoGame> ApplyFiltersAndSorting(IQueryable<MongoGame> query)
    {
        query = _steps.Aggregate(query, (current, step) => step.Process(current));
        query = _sortingStep.OrderGames(query);
        return query;
    }

    // Applies pagination only
    public IQueryable<Game> ApplyPagination(IQueryable<Game> query)
        => _paginationStep.Paginate(query);

    public IQueryable<MongoGame> ApplyPagination(IQueryable<MongoGame> query)
        => _paginationStep.Paginate(query);

    public IEnumerable<Game> ResortAndRepaginate(IEnumerable<Game> games)
    {
        games = _sortingStep.OrderGames(games.AsQueryable()).ToList();
        games = _paginationStep.RepaginateGames(games);
        return games;
    }
}