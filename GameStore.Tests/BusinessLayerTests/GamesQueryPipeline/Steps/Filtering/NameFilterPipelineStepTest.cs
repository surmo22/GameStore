using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Steps.Filtering;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.GamesQueryPipeline.Steps.Filtering;

public class NameFilterPipelineStepTests
{
    [Fact]
    public void Process_GameQuery_WithName_FiltersCorrectly()
    {
        // Arrange
        var filterRequest = new GameFilterRequest { Name = "Zelda" };
        var step = new NameFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { Name = "Legend of Zelda" },
            new() { Name = "Super Mario" }
        }.AsQueryable();

        // Act
        var result = step.Process(games).ToList();

        // Assert
        Assert.Single(result);
        Assert.Contains("Zelda", result[0].Name);
    }

    [Fact]
    public void Process_GameQuery_EmptyName_ReturnsUnfiltered()
    {
        // Arrange
        var filterRequest = new GameFilterRequest { Name = "" };
        var step = new NameFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { Name = "Donkey Kong" }
        }.AsQueryable();

        // Act
        var result = step.Process(games).ToList();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public void Process_MongoGameQuery_WithName_FiltersCorrectly()
    {
        // Arrange
        var filterRequest = new GameFilterRequest { Name = "Halo" };
        var step = new NameFilterPipelineStep(filterRequest);

        var games = new List<MongoGame>
        {
            new() { ProductName = "Halo Infinite" },
            new() { ProductName = "Forza Horizon" }
        }.AsQueryable();

        // Act
        var result = step.Process(games).ToList();

        // Assert
        Assert.Single(result);
        Assert.Contains("Halo", result[0].ProductName);
    }

    [Fact]
    public void Process_MongoGameQuery_EmptyName_ReturnsUnfiltered()
    {
        // Arrange
        var filterRequest = new GameFilterRequest { Name = null };
        var step = new NameFilterPipelineStep(filterRequest);

        var games = new List<MongoGame>
        {
            new() { ProductName = "Minecraft" }
        }.AsQueryable();

        // Act
        var result = step.Process(games).ToList();

        // Assert
        Assert.Single(result);
    }
}
