using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Steps.Filtering;
using GameStore.Common.Utils;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.GamesQueryPipeline.Steps.Filtering;

public class PlatformFilterPipelineStepTests
{
    [Fact]
    public void Process_GameQuery_WithPlatforms_FiltersCorrectly()
    {
        // Arrange
        var id = 1;
        var guid = IntToGuidConverter.Convert(id);
        var filterRequest = new GameFilterRequest { Platforms = new List<Guid> { guid } };
        var step = new PlatformFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { Platforms = new List<Platform> { new() { Id = guid } } },
            new() { Platforms = new List<Platform> { new() { Id = Guid.NewGuid() } } }
        }.AsQueryable();

        // Act
        var result = step.Process(games).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(guid, result[0].Platforms.First().Id);
    }

    [Fact]
    public void Process_GameQuery_EmptyPlatforms_ReturnsUnfiltered()
    {
        // Arrange
        var filterRequest = new GameFilterRequest();
        var step = new PlatformFilterPipelineStep(filterRequest);
        
        var games = new List<Game>
        {
            new() { Platforms = new List<Platform> { new() { Id = Guid.NewGuid() } } }
        }.AsQueryable();

        // Act
        var result = step.Process(games).ToList();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public void Process_MongoGameQuery_WithPlatforms_ReturnsEmpty()
    {
        // Arrange
        var filterRequest = new GameFilterRequest { Platforms = new List<Guid> { Guid.NewGuid() } };
        var step = new PlatformFilterPipelineStep(filterRequest);

        var mongoGames = new List<MongoGame>
        {
            new() { ProductName = "Minecraft" }
        }.AsQueryable();

        // Act
        var result = step.Process(mongoGames).ToList();

        // Assert
        Assert.Empty(result); // Platform filter disables MongoGame results
    }

    [Fact]
    public void Process_MongoGameQuery_EmptyPlatforms_ReturnsUnfiltered()
    {
        // Arrange
        var filterRequest = new GameFilterRequest(); // No platform filter
        var step = new PlatformFilterPipelineStep(filterRequest);

        var mongoGames = new List<MongoGame>
        {
            new() { ProductName = "Roblox" }
        }.AsQueryable();

        // Act
        var result = step.Process(mongoGames).ToList();

        // Assert
        Assert.Single(result);
    }
}
