using FluentAssertions;
using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Services.GameServices.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.StrategiesTests;

public class CreationDateSetupStepTest
{
    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
    private readonly CreationDateSetupStep _step;

    public CreationDateSetupStepTest()
    {
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();
        _step = new CreationDateSetupStep(_mockDateTimeProvider.Object);
    }

    [Fact]
    public void InitializeField_ShouldSetCreationDateToCurrentUtcNow()
    {
        // Arrange
        var game = new Game();
        var mockGameRequest = new Mock<IGameRequest>();

        var currentTime = new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Utc);
        _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(currentTime);

        // Act
        _step.InitializeField(game, mockGameRequest.Object);

        // Assert
        game.CreationDate.Should().Be(currentTime);
        _mockDateTimeProvider.Verify(m => m.UtcNow, Times.Once);
    }

    [Fact]
    public void InitializeField_ShouldNotThrow_WhenGameRequestIsNull()
    {
        // Arrange
        var game = new Game();

        var currentTime = new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Utc);
        _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(currentTime);

        // Act
        var act = () => _step.InitializeField(game, null);

        // Assert
        act.Should().NotThrow();
        game.CreationDate.Should().Be(currentTime);
        _mockDateTimeProvider.Verify(m => m.UtcNow, Times.Once);
    }

    [Fact]
    public void InitializeField_ShouldPreservePrecisionOfDateTime()
    {
        // Arrange
        var game = new Game();
        var mockGameRequest = new Mock<IGameRequest>();

        var preciseTime = new DateTime(2023, 10, 1, 12, 0, 0, 123, DateTimeKind.Utc);
        _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(preciseTime);

        // Act
        _step.InitializeField(game, mockGameRequest.Object);

        // Assert
        game.CreationDate.Should().Be(preciseTime);
        _mockDateTimeProvider.Verify(m => m.UtcNow, Times.Once);
    }
}