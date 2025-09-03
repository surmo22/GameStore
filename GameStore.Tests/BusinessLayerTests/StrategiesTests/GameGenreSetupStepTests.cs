using GameStore.BLL.DTOs.Games;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Services.GameServices.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.StrategiesTests;

public class GameGenreSetupStepTests
{
    [Fact]
    public async Task InitializeFieldAsync_ShouldSetGenres()
    {
        // Arrange
        var genreServiceMock = new Mock<IGenreService>();
        var game = new Game();
        var guids = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var gameRequest = new GameCreateRequestDto
        {
            Genres = guids,
        };
        var cancellationToken = new CancellationToken(false);

        var genres = new List<Genre> { new(), new(), new() };
        genreServiceMock.Setup(x => x.GetGenresByIdsAsync(It.IsAny<List<Guid>>(), cancellationToken))
            .ReturnsAsync(genres);

        var gameGenreSetupStrategy = new GameGenreSetupStep(genreServiceMock.Object);

        // Act
        await gameGenreSetupStrategy.InitializeFieldAsync(game, gameRequest, cancellationToken);

        // Assert
        Assert.Equal(3, game.Genres.Count);
    }
}