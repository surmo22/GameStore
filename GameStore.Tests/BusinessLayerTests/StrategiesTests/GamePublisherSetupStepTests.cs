using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Publisher;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Services.GameServices.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.StrategiesTests;

public class GamePublisherSetupStepTests
{
    private readonly GamePublisherSetupStep _testClass;
    private readonly Mock<IPublisherService> _publisherServiceMock = new();

    public GamePublisherSetupStepTests()
    {
        _testClass = new GamePublisherSetupStep(_publisherServiceMock.Object);
    }

    [Fact]
    public async Task SetupPublisherField_SetsPublisherId_WhenPublisherExist()
    {
        // Arrange
        var game = new Game();
        var gameRequest = new GameCreateRequestDto()
        {
            PublisherId = Guid.NewGuid(),
        };
        var publisher = new Publisher()
        {
            Id = Guid.NewGuid(),
        };
        _publisherServiceMock.Setup(p => p.GetPublisherByIdNotMappedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(publisher);

        // Act
        await _testClass.InitializeFieldAsync(game, gameRequest, CancellationToken.None);

        // Assert
        _publisherServiceMock.Verify(p => p.GetPublisherByIdNotMappedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(gameRequest.PublisherId, game.PublisherId);
    }

    [Fact]
    public async Task SetupPublisherId_ThrowsKeyNotFoundException_WhenPublisherNotFound()
    {
        // Arrange
        var game = new Game();
        var gameRequest = new GameCreateRequestDto()
        {
            PublisherId = Guid.NewGuid(),
        };
        var cancellationToken = CancellationToken.None;
        _publisherServiceMock.Setup(p => p.GetPublisherByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PublisherDto)null!);

        // Act && Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _testClass.InitializeFieldAsync(game, gameRequest, cancellationToken));
    }
}