using GameStore.BLL.GamesQueryPipeline;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.GamesQueryPipeline;

public class GamePipelineTests
{
    [Fact]
    public void Execute_ForGamePipeline_AppliesStepsSortingAndPagination()
    {
        // Arrange
        var data = new List<Game>
        {
            new() { Name = "Game A", Price = 10 },
            new() { Name = "Game B", Price = 20 }
        }.AsQueryable();

        var mockStep = new Mock<IGamePipelineStep>();
        mockStep.Setup(s => s.Process(It.IsAny<IQueryable<Game>>()))
                .Returns<IQueryable<Game>>(q => q.Where(g => g.Price >= 15));

        var mockSort = new Mock<IGamePipelineOrderingStep>();
        mockSort.Setup(s => s.OrderGames(It.IsAny<IQueryable<Game>>()))
                .Returns<IQueryable<Game>>(q => q.OrderByDescending(g => g.Price));

        var mockPagination = new Mock<IGamePipelinePaginationStep>();
        mockPagination.Setup(p => p.Paginate(It.IsAny<IQueryable<Game>>()))
                      .Returns<IQueryable<Game>>(q => q.Take(1));

        var pipeline = new GamePipeline()
            .AddStep(mockStep.Object)
            .WithSorting(mockSort.Object)
            .WithPagination(mockPagination.Object);

        // Act
        var result = pipeline.Execute(data).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Game B", result[0].Name);
    }

    [Fact]
    public void Execute_ForMongoGamePipeline_AppliesStepsSortingAndPagination()
    {
        // Arrange
        var mongoData = new List<MongoGame>
        {
            new() { ProductName = "Mongo A" },
            new() { ProductName = "Mongo B" }
        }.AsQueryable();

        var mockStep = new Mock<IGamePipelineStep>();
        mockStep.Setup(s => s.Process(It.IsAny<IQueryable<MongoGame>>()))
                .Returns<IQueryable<MongoGame>>(q => 
                    q.Where(g => g.ProductName.Contains('B')));

        var mockSort = new Mock<IGamePipelineOrderingStep>();
        mockSort.Setup(s => s.OrderGames(It.IsAny<IQueryable<MongoGame>>()))
                .Returns<IQueryable<MongoGame>>(q => q);

        var mockPagination = new Mock<IGamePipelinePaginationStep>();
        mockPagination.Setup(p => p.Paginate(It.IsAny<IQueryable<MongoGame>>()))
                      .Returns<IQueryable<MongoGame>>(q => q);

        var pipeline = new GamePipeline()
            .AddStep(mockStep.Object)
            .WithSorting(mockSort.Object)
            .WithPagination(mockPagination.Object);

        // Act
        var result = pipeline.Execute(mongoData).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Mongo B", result[0].ProductName);
    }

    [Fact]
    public void ResortAndRepaginate_OrdersThenRepaginates()
    {
        // Arrange
        var games = new List<Game>
        {
            new() { Name = "Game A", Price = 10 },
            new() { Name = "Game B", Price = 20 },
            new() { Name = "Game C", Price = 30 }
        };

        var mockSort = new Mock<IGamePipelineOrderingStep>();
        mockSort.Setup(s => s.OrderGames(It.IsAny<IQueryable<Game>>()))
                .Returns<IQueryable<Game>>(q => q.OrderByDescending(g => g.Price));

        var mockPagination = new Mock<IGamePipelinePaginationStep>();
        mockPagination.Setup(p => p.RepaginateGames(It.IsAny<IEnumerable<Game>>()))
                      .Returns<IEnumerable<Game>>(g => g.Take(2));

        var pipeline = new GamePipeline()
            .WithSorting(mockSort.Object)
            .WithPagination(mockPagination.Object);

        // Act
        var result = pipeline.ResortAndRepaginate(games).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Game C", result[0].Name);
        Assert.Equal("Game B", result[1].Name);
    }
}
