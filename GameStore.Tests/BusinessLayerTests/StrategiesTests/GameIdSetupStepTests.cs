using GameStore.BLL.DTOs.Games;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Services.GameServices.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.StrategiesTests;

public class GameIdSetupStepTests
{
    private readonly Mock<IGuidProvider> _guidProvider = new();

    public GameIdSetupStepTests()
    {
        _guidProvider.Setup(x => x.NewGuid()).Returns(Guid.NewGuid());
    }
    
    [Fact]
    public void InitializeFieldAsync_ShouldSetId()
    {
        // Arrange
        var game = new Game();
        var gameRequest = new GameCreateRequestDto();

        var gameIdSetupStrategy = new GameIdSetupStep(_guidProvider.Object);

        // Act
        gameIdSetupStrategy.InitializeField(game, gameRequest);

        // Assert
        Assert.NotEqual(game.Id, Guid.Empty);
    }

    [Fact]
    public void InitializeField_ShouldNotChangeKey_WhenUpdateRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        var game = new Game
        {
            Id = id,
        };
        var gameRequest = new GameUpdateRequestDto();
        var gameIdSetupStrategy = new GameIdSetupStep(_guidProvider.Object);

        // Act
        gameIdSetupStrategy.InitializeField(game, gameRequest);

        // Assert
        Assert.Equal(id, game.Id);
    }
}