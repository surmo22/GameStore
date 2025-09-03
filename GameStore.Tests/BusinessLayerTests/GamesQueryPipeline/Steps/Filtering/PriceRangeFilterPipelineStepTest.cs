using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Steps.Filtering;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.GamesQueryPipeline.Steps.Filtering;

public class PriceRangeFilterPipelineStepTests
{
    [Fact]
    public void Process_GameQuery_WithMinPrice_FiltersCorrectly()
    {
        var filterRequest = new GameFilterRequest { MinPrice = 20 };
        var step = new PriceRangeFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { Price = 1 },
            new() { Price = 20 }
        }.AsQueryable();

        var result = step.Process(games).ToList();

        Assert.Single(result);
        Assert.Equal(20, result[0].Price);
    }

    [Fact]
    public void Process_GameQuery_WithMaxPrice_FiltersCorrectly()
    {
        var filterRequest = new GameFilterRequest { MaxPrice = 2 };
        var step = new PriceRangeFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { Price = 1 },
            new() { Price = 4 }
        }.AsQueryable();

        var result = step.Process(games).ToList();

        Assert.Single(result);
        Assert.Equal(1, result[0].Price);
    }

    [Fact]
    public void Process_GameQuery_WithMinAndMaxPrice_FiltersInRange()
    {
        var filterRequest = new GameFilterRequest { MinPrice = 1, MaxPrice = 3 };
        var step = new PriceRangeFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { Price = 0 },
            new() { Price = 2 },
            new() { Price = 4 }
        }.AsQueryable();

        var result = step.Process(games).ToList();

        Assert.Single(result);
        Assert.Equal(2, result[0].Price);
    }

    [Fact]
    public void Process_GameQuery_NoPriceFilter_ReturnsAll()
    {
        var filterRequest = new GameFilterRequest();
        var step = new PriceRangeFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { Price = 1 },
            new() { Price = 2 }
        }.AsQueryable();

        var result = step.Process(games).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Process_MongoGameQuery_WithMinAndMaxPrice_FiltersInRange()
    {
        var filterRequest = new GameFilterRequest { MinPrice = 1, MaxPrice = 3 };
        var step = new PriceRangeFilterPipelineStep(filterRequest);

        var mongoGames = new List<MongoGame>
        {
            new() { UnitPrice = 2 },
            new() { UnitPrice = 5 },
            new() { UnitPrice = 4 }
        }.AsQueryable();

        var result = step.Process(mongoGames).ToList();

        Assert.Single(result);
        Assert.Equal(2, result[0].UnitPrice);
    }

    [Fact]
    public void Process_MongoGameQuery_OnlyMinPrice_FiltersCorrectly()
    {
        var filterRequest = new GameFilterRequest { MinPrice = 20 };
        var step = new PriceRangeFilterPipelineStep(filterRequest);

        var mongoGames = new List<MongoGame>
        {
            new() { UnitPrice = 15 },
            new() { UnitPrice = 25 }
        }.AsQueryable();

        var result = step.Process(mongoGames).ToList();

        Assert.Single(result);
        Assert.Equal(25, result[0].UnitPrice);
    }

    [Fact]
    public void Process_MongoGameQuery_NoPriceFilter_ReturnsAll()
    {
        var filterRequest = new GameFilterRequest();
        var step = new PriceRangeFilterPipelineStep(filterRequest);

        var mongoGames = new List<MongoGame>
        {
            new() { UnitPrice = 5 },
            new() { UnitPrice = 100 }
        }.AsQueryable();

        var result = step.Process(mongoGames).ToList();

        Assert.Equal(2, result.Count);
    }
}
