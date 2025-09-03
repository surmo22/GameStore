using GameStore.BLL.GamesQueryPipeline.Steps.Filtering;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using JetBrains.Annotations;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.GamesQueryPipeline.Steps.Filtering;

[TestSubject(typeof(NotIncludeDeletedGamesPipelineStep))]
public class NotIncludeDeletedGamesPipelineStepTest
{
    private readonly NotIncludeDeletedGamesPipelineStep _step = new();

    [Fact]
    public void Process_GameQuery_FiltersOutDeletedGames()
    {
        // Arrange
        var games = new List<Game>
        {
            new() { IsDeleted = false },
            new() { IsDeleted = true },
            new() { IsDeleted = false }
        }.AsQueryable();

        // Act
        var result = _step.Process(games);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, g => Assert.False(g.IsDeleted));
    }

    [Fact]
    public void Process_GameQuery_AllDeleted_ReturnsEmpty()
    {
        // Arrange
        var games = new List<Game>
        {
            new() { IsDeleted = true },
            new() { IsDeleted = true }
        }.AsQueryable();

        // Act
        var result = _step.Process(games);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Process_GameQuery_NoneDeleted_ReturnsAll()
    {
        // Arrange
        var games = new List<Game>
        {
            new() { IsDeleted = false },
            new() { IsDeleted = false }
        }.AsQueryable();

        // Act
        var result = _step.Process(games);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void Process_MongoGameQuery_ReturnsUnchanged()
    {
        // Arrange
        var mongoGames = new List<MongoGame>
        {
            new(),
            new()
        }.AsQueryable();

        // Act
        var result = _step.Process(mongoGames);

        // Assert
        Assert.Equal(mongoGames, result); // reference equality is fine for IQueryable
    }
}