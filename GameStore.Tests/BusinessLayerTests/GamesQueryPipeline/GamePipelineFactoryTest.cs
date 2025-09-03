using System.Reflection;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.BLL.GamesQueryPipeline.Steps.Filtering;
using GameStore.BLL.GamesQueryPipeline.Steps.Pagination;
using GameStore.BLL.GamesQueryPipeline.Steps.Sorting;
using GameStore.Common.Constants;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.GamesQueryPipeline;

public class GamePipelineFactoryTests
{
    [Fact]
    public void Create_ReturnsGamePipeline_WithAllStepsConfigured()
    {
        // Arrange
        var request = new GameFilterRequest
        {
            Genres = [Guid.NewGuid(), Guid.NewGuid()],
            Name = "Test",
            Publisher = Guid.NewGuid(),
            Platforms = [Guid.NewGuid(), Guid.NewGuid()],
            MinPrice = 10,
            MaxPrice = 100,
            DatePublishing = PublishingDateFilter.LastMonth,
            Sort = SortingOptions.PriceAscending,
            PageCount = "2",
            Page = 1
        };

        var factory = new GamePipelineFactory();

        // Act
        var pipeline = factory.Create(request, true);

        // Assert
        Assert.NotNull(pipeline);
        Assert.IsType<GamePipeline>(pipeline);
    }

    [Fact]
    public void Create_ContainsPaginationStep()
    {
        // Arrange
        var request = new GameFilterRequest();
        var factory = new GamePipelineFactory();

        // Act
        var pipeline = factory.Create(request, true);

        // Assert
        var paginationStepField = pipeline.GetType().GetField("_paginationStep", BindingFlags.NonPublic | BindingFlags.Instance);
        var paginationStep = paginationStepField?.GetValue(pipeline);

        Assert.NotNull(paginationStep);
        Assert.IsType<PaginationPipelineStep>(paginationStep);
    }

    [Fact]
    public void Create_ContainsSortingStep()
    {
        // Arrange
        var request = new GameFilterRequest();
        var factory = new GamePipelineFactory();

        // Act
        var pipeline = factory.Create(request, false);

        // Assert
        var sortingStepField = pipeline.GetType().GetField("_sortingStep", BindingFlags.NonPublic | BindingFlags.Instance);
        var sortingStep = sortingStepField?.GetValue(pipeline);

        Assert.NotNull(sortingStep);
        Assert.IsType<OrderingPipelineStep>(sortingStep);
    }

    [Fact]
    public void Create_ContainsAllSteps()
    {
        // Arrange
        var request = new GameFilterRequest();
        var factory = new GamePipelineFactory();

        // Act
        var pipeline = factory.Create(request, true);

        // Assert that the pipeline contains the expected steps
        AssertStepType(pipeline, typeof(GenresFilterPipelineStep));
        AssertStepType(pipeline, typeof(NameFilterPipelineStep));
        AssertStepType(pipeline, typeof(PublisherFilterPipelineStep));
        AssertStepType(pipeline, typeof(PlatformFilterPipelineStep));
        AssertStepType(pipeline, typeof(PriceRangeFilterPipelineStep));
        AssertStepType(pipeline, typeof(PublishingDateFilterPipelineStep));
    }

    private static void AssertStepType(IGamePipeline pipeline, Type stepType)
    {
        var stepsField = pipeline.GetType().GetField("_steps", BindingFlags.NonPublic | BindingFlags.Instance);
        var steps = (IList<IGamePipelineStep>)stepsField?.GetValue(pipeline);

        Assert.Contains(steps!, step => step.GetType() == stepType);
    }
}
