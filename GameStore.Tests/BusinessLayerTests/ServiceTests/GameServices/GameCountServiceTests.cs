using GameStore.BLL.Services.GameServices;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.GameServices;

public class GameCountServiceTests
{
    [Fact]
    public void GetTotalGamesCount_ShouldReturnTotalGamesCount()
    {
        // Arrange
        var gameCountService = new GameCountService(10);

        // Act
        var result = gameCountService.GetTotalGamesCount();

        // Assert
        Assert.Equal(10, result);
    }

    [Fact]
    public void IncrementTotalGamesCount_ShouldIncrementTotalGamesCount()
    {
        // Arrange
        var gameCountService = new GameCountService(10);

        // Act
        var result = gameCountService.IncrementTotalGamesCount();

        // Assert
        Assert.Equal(11, result);
    }

    [Fact]
    public void DecrementTotalGamesCount_ShouldDecrementTotalGamesCount()
    {
        // Arrange
        var gameCountService = new GameCountService(10);

        // Act
        var result = gameCountService.DecrementTotalGamesCount();

        // Assert
        Assert.Equal(9, result);
    }

    [Fact]
    public void ConstructorThrowsException_WhenNumberIsLessThanZero()
    {
        // Arrange && Act && Assert
        Assert.Throws<InvalidOperationException>(() => new GameCountService(-1));
    }
}