using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Steps.Sorting;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.GamesQueryPipeline.Steps.Sorting;

public class OrderingPipelineStepTests
{
    [Fact]
    public void OrderGames_Game_MostPopular_SortsDescendingByViewCount()
    {
        var request = new GameFilterRequest { Sort = SortingOptions.MostPopular };
        var step = new OrderingPipelineStep(request);

        var games = new List<Game>
        {
            new() { ViewCount = 10 },
            new() { ViewCount = 50 }
        }.AsQueryable();

        var result = step.OrderGames(games).ToList();

        Assert.Equal(50, result[0].ViewCount);
    }

    [Fact]
    public void OrderGames_Game_MostCommented_SortsDescendingByCommentCount()
    {
        var request = new GameFilterRequest { Sort = SortingOptions.MostCommented };
        var step = new OrderingPipelineStep(request);

        var games = new List<Game>
        {
            new() { Comments = new List<Comment> { new() } },
            new() { Comments = new List<Comment> { new(), new(), new() } }
        }.AsQueryable();

        var result = step.OrderGames(games).ToList();

        Assert.Equal(3, result[0].Comments.Count);
    }

    [Fact]
    public void OrderGames_Game_PriceAscending_SortsByPriceLowToHigh()
    {
        var request = new GameFilterRequest { Sort = SortingOptions.PriceAscending };
        var step = new OrderingPipelineStep(request);

        var games = new List<Game>
        {
            new() { Price = 100 },
            new() { Price = 50 }
        }.AsQueryable();

        var result = step.OrderGames(games).ToList();

        Assert.Equal(50, result[0].Price);
    }

    [Fact]
    public void OrderGames_Game_PriceDescending_SortsByPriceHighToLow()
    {
        var request = new GameFilterRequest { Sort = SortingOptions.PriceDescending };
        var step = new OrderingPipelineStep(request);

        var games = new List<Game>
        {
            new() { Price = 20 },
            new() { Price = 200 }
        }.AsQueryable();

        var result = step.OrderGames(games).ToList();

        Assert.Equal(200, result[0].Price);
    }

    [Fact]
    public void OrderGames_Game_New_SortsByCreationDateDescending()
    {
        var request = new GameFilterRequest { Sort = SortingOptions.New };
        var step = new OrderingPipelineStep(request);

        var now = DateTime.Now;
        var games = new List<Game>
        {
            new() { CreationDate = now.AddDays(-10) },
            new() { CreationDate = now }
        }.AsQueryable();

        var result = step.OrderGames(games).ToList();

        Assert.True(result[0].CreationDate > result[1].CreationDate);
    }

    [Fact]
    public void OrderGames_Mongo_MostPopular_SortsByViewCount()
    {
        var request = new GameFilterRequest { Sort = SortingOptions.MostPopular };
        var step = new OrderingPipelineStep(request);

        var mongoGames = new List<MongoGame>
        {
            new() { ViewCount = 1 },
            new() { ViewCount = 99 }
        }.AsQueryable();

        var result = step.OrderGames(mongoGames).ToList();

        Assert.Equal(1, result[0].ViewCount); // Ascending in mongo
    }

    [Fact]
    public void OrderGames_Mongo_MostCommented_DoesNotSort()
    {
        var request = new GameFilterRequest { Sort = SortingOptions.MostCommented };
        var step = new OrderingPipelineStep(request);

        var mongoGames = new List<MongoGame>
        {
            new() { ViewCount = 10 },
            new() { ViewCount = 20 }
        }.AsQueryable();

        var result = step.OrderGames(mongoGames).ToList();

        Assert.Equal(10, result[0].ViewCount); // Order unchanged
    }

    [Fact]
    public void OrderGames_Mongo_PriceAscending_SortsByUnitPriceAsc()
    {
        var request = new GameFilterRequest { Sort = SortingOptions.PriceAscending };
        var step = new OrderingPipelineStep(request);

        var mongoGames = new List<MongoGame>
        {
            new() { UnitPrice = 99 },
            new() { UnitPrice = 10 }
        }.AsQueryable();

        var result = step.OrderGames(mongoGames).ToList();

        Assert.Equal(10, result[0].UnitPrice);
    }

    [Fact]
    public void OrderGames_Mongo_PriceDescending_SortsByUnitPriceDesc()
    {
        var request = new GameFilterRequest { Sort = SortingOptions.PriceDescending };
        var step = new OrderingPipelineStep(request);

        var mongoGames = new List<MongoGame>
        {
            new() { UnitPrice = 5 },
            new() { UnitPrice = 50 }
        }.AsQueryable();

        var result = step.OrderGames(mongoGames).ToList();

        Assert.Equal(50, result[0].UnitPrice);
    }

    [Fact]
    public void OrderGames_Mongo_New_DoesNotSort()
    {
        var request = new GameFilterRequest { Sort = SortingOptions.New };
        var step = new OrderingPipelineStep(request);

        var mongoGames = new List<MongoGame>
        {
            new() { UnitPrice = 1 },
            new() { UnitPrice = 2 }
        }.AsQueryable();

        var result = step.OrderGames(mongoGames).ToList();

        Assert.Equal(1, result[0].UnitPrice); // Order unchanged
    }

    [Fact]
    public void OrderGames_Game_EmptySort_ReturnsUnchanged()
    {
        var request = new GameFilterRequest { Sort = null };
        var step = new OrderingPipelineStep(request);

        var games = new List<Game>
        {
            new() { Price = 100 },
            new() { Price = 50 }
        }.AsQueryable();

        var result = step.OrderGames(games).ToList();

        Assert.Equal(100, result[0].Price);
    }
}
