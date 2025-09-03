using GameStore.BLL.DTOs.Games;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Services.GameServices.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.StrategiesTests;

public class GamePlatformSetupStepTests
{
    [Fact]
    public async Task InitializeFieldAsync_ShouldSetPlatforms()
    {
        // Arrange
        var platformServiceMock = new Mock<IPlatformService>();
        var game = new Game();
        var guids = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var gameRequest = new GameCreateRequestDto
        {
            Platforms = guids,
        };
        var cancellationToken = new CancellationToken(false);

        var platforms = new List<Platform> { new(), new(), new() };
        platformServiceMock.Setup(x => x.GetPlatformsByIdsAsync(It.IsAny<List<Guid>>(), cancellationToken))
            .ReturnsAsync(platforms);

        var gamePlatformSetupStrategy = new GamePlatformSetupStep(platformServiceMock.Object);

        // Act
        await gamePlatformSetupStrategy.InitializeFieldAsync(game, gameRequest, cancellationToken);

        // Assert
        Assert.Equal(3, game.Platforms.Count);
    }
}