using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Steps.Filtering;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.GamesQueryPipeline.Steps.Filtering;

public class PublishingDateFilterPipelineStepTests
{
    [Fact]
    public void Process_GameQuery_WithValidDatePublishing_FiltersCorrectly()
    {
        var filterRequest = new GameFilterRequest { DatePublishing = PublishingDateFilter.LastWeek };
        var step = new PublishingDateFilterPipelineStep(filterRequest);

        var today = DateTime.Today;
        var oneWeekAgo = today.AddDays(-7);

        var games = new List<Game>
        {
            new() { CreationDate = today },
            new() { CreationDate = today.AddDays(-10) }
        }.AsQueryable();

        var result = step.Process(games).ToList();

        Assert.Single(result);
        Assert.True(result[0].CreationDate >= oneWeekAgo);
    }

    [Fact]
    public void Process_GameQuery_NullDatePublishing_ReturnsUnfiltered()
    {
        var filterRequest = new GameFilterRequest { DatePublishing = null };
        var step = new PublishingDateFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { CreationDate = DateTime.Today.AddDays(-200) }
        }.AsQueryable();

        var result = step.Process(games).ToList();

        Assert.Single(result);
    }

    [Fact]
    public void Process_MongoGameQuery_WithDatePublishing_ReturnsEmpty()
    {
        var filterRequest = new GameFilterRequest { DatePublishing = PublishingDateFilter.LastWeek };
        var step = new PublishingDateFilterPipelineStep(filterRequest);

        var mongoGames = new List<MongoGame>
        {
            new() { ProductName = "Anything" }
        }.AsQueryable();

        var result = step.Process(mongoGames).ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void Process_MongoGameQuery_EmptyDatePublishing_ReturnsUnfiltered()
    {
        var filterRequest = new GameFilterRequest { DatePublishing = null };
        var step = new PublishingDateFilterPipelineStep(filterRequest);

        var mongoGames = new List<MongoGame>
        {
            new() { ProductName = "A Game" }
        }.AsQueryable();

        var result = step.Process(mongoGames).ToList();

        Assert.Single(result);
    }
}
