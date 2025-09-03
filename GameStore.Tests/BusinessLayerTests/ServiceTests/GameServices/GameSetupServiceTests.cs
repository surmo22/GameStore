using GameStore.BLL.DTOs.Games;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.GameSetupStrategies;
using GameStore.BLL.Services.GameServices;
using GameStore.BLL.Services.GameServices.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.GameServices;

public class GameSetupServiceTests
{
    [Fact]
    public async Task BuildGameAsync_ShouldReturnCompletedGame()
    {
        // Arrange
        var game = new Game
        {
            Name = "Test Game",
            Description = "Test Description",
        };
        var gameCreateDto = new GameCreateDto
        {
            Name = "Test Game",
            Description = "Test Description",
        };
        var listIds = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var gameRequest = new GameCreateRequestDto
        {
            Game = gameCreateDto,
            Genres = listIds,
            Platforms = listIds,
        };
        var cancellationToken = new CancellationToken(false);
        var genreServiceMock = new Mock<IGenreService>();
        var platformServiceMock = new Mock<IPlatformService>();
        var guidProvider = new Mock<IGuidProvider>();
        guidProvider.Setup(x => x.NewGuid()).Returns(Guid.NewGuid);

        var strategies = new List<IGameSetupStep>
        {
            new GameIdSetupStep(guidProvider.Object),
            new GameKeySetupStep(),
            new GameGenreSetupStep(genreServiceMock.Object),
            new GamePlatformSetupStep(platformServiceMock.Object),
        };

        genreServiceMock.Setup(x => x.GetGenresByIdsAsync(It.IsAny<List<Guid>>(), cancellationToken))
            .ReturnsAsync([new Genre(), new Genre(), new Genre()]);
        platformServiceMock.Setup(x => x.GetPlatformsByIdsAsync(It.IsAny<List<Guid>>(), cancellationToken))
            .ReturnsAsync([new Platform(), new Platform(), new Platform()]);

        var gameSetupService = new GameSetupService(strategies);

        // Act
        await gameSetupService.BuildGameAsync(game, gameRequest, cancellationToken);

        // Assert
        Assert.NotEqual(game.Id, Guid.Empty);
        Assert.NotNull(game.Key);
        Assert.NotNull(game.Genres);
        Assert.NotNull(game.Platforms);
        Assert.Equal(game.Key, game.Name);
        Assert.Equal(3, game.Genres.Count);
        Assert.Equal(3, game.Platforms.Count);
    }
}