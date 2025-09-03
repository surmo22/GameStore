using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Steps.Filtering;
using GameStore.Common.Utils;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.GamesQueryPipeline.Steps.Filtering;

public class GenresFilterPipelineStepTests
{
    [Fact]
    public void Process_GameQuery_WithGenres_FiltersCorrectly()
    {
        // Arrange
        var id = 1;
        var guid = IntToGuidConverter.Convert(id);
        var filterRequest = new GameFilterRequest { Genres = new List<Guid> { guid } };
        var step = new GenresFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { Genres = new List<Genre> { new() { Id = guid } } },
            new() { Genres = new List<Genre> { new() { Id = Guid.NewGuid() } } }
        }.AsQueryable();

        // Act
        var result = step.Process(games).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(guid, result[0].Genres.First().Id);
    }

    [Fact]
    public void Process_GameQuery_EmptyGenres_ReturnsUnfiltered()
    {
        // Arrange
        var filterRequest = new GameFilterRequest();
        var step = new GenresFilterPipelineStep(filterRequest);

        var games = new List<Game>
        {
            new() { Genres = new List<Genre> { new() { Id = Guid.NewGuid() } } }
        }.AsQueryable();

        // Act
        var result = step.Process(games).ToList();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public void Process_MongoGameQuery_WithGenres_FiltersCorrectly()
    {
        // Arrange
        var genreId = 123;
        var guidId = IntToGuidConverter.Convert(genreId);

        var filterRequest = new GameFilterRequest { Genres = [guidId] };
        
        var step = new GenresFilterPipelineStep(filterRequest);

        var games = new List<MongoGame>
        {
            new() { CategoryId = genreId },
            new() { CategoryId = 111 }
        }.AsQueryable();

        // Act
        var result = step.Process(games).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(genreId, result[0].CategoryId);
    }

    [Fact]
    public void Process_MongoGameQuery_EmptyGenres_ReturnsUnfiltered()
    {
        // Arrange
        var filterRequest = new GameFilterRequest();
        var step = new GenresFilterPipelineStep(filterRequest);

        var games = new List<MongoGame>
        {
            new() { CategoryId = 11 }
        }.AsQueryable();

        // Act
        var result = step.Process(games).ToList();

        // Assert
        Assert.Single(result);
    }
}