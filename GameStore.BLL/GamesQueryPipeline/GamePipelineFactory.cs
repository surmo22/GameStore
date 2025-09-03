using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.BLL.GamesQueryPipeline.Steps.Filtering;
using GameStore.BLL.GamesQueryPipeline.Steps.Pagination;
using GameStore.BLL.GamesQueryPipeline.Steps.Sorting;

namespace GameStore.BLL.GamesQueryPipeline;

public class GamePipelineFactory : IGamePipelineFactory
{
    public IGamePipeline Create(GameFilterRequest filterRequest, bool hasPermissionToViewDeletedGames)
    {
        var pipeline =  new GamePipeline()
            .AddStep(new GenresFilterPipelineStep(filterRequest))
            .AddStep(new NameFilterPipelineStep(filterRequest))
            .AddStep(new PublisherFilterPipelineStep(filterRequest))
            .AddStep(new PlatformFilterPipelineStep(filterRequest))
            .AddStep(new PriceRangeFilterPipelineStep(filterRequest))
            .AddStep(new PublishingDateFilterPipelineStep(filterRequest))
            .WithSorting(new OrderingPipelineStep(filterRequest))
            .WithPagination(new PaginationPipelineStep(filterRequest));

        if (!hasPermissionToViewDeletedGames)
        {
            pipeline.AddStep(new NotIncludeDeletedGamesPipelineStep());
        }
        
        return pipeline;
    }
}