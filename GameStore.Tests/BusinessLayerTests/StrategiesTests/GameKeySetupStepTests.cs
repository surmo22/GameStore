using GameStore.BLL.DTOs.Games;
using GameStore.BLL.Services.GameServices.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.StrategiesTests;

public class GameKeySetupStepTests
{
    [Fact]
    public void InitializeFieldAsync_ShouldSetKey()
    {
        // Arrange
        var game = new Game
        {
            Name = "Test Game",
        };
        var gameRequest = new GameCreateRequestDto();

        var gameKeySetupStrategy = new GameKeySetupStep();

        // Act
        gameKeySetupStrategy.InitializeField(game, gameRequest);

        // Assert
        Assert.NotNull(game.Key);
        Assert.Equal(game.Key, game.Name);
    }

    [Fact]
    public void InitializeField_ShouldNotChangeKey_WhenAlreadySet()
    {
        // Arrange
        const string key = "Test Key";
        var game = new Game
        {
            Key = key,
        };
        var gameRequest = new GameCreateRequestDto();
        var gameKeySetupStrategy = new GameKeySetupStep();

        // Act
        gameKeySetupStrategy.InitializeField(game, gameRequest);

        // Assert
        Assert.NotNull(game.Key);
        Assert.Equal(key, game.Key);
    }
}