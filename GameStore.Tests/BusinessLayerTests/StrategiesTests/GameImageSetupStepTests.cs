using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces.GameServices.ImageServices;
using GameStore.BLL.Services.GameServices.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.StrategiesTests;

public class GameImageSetupStepTests
{
    [Fact]
    public async Task InitializeFieldAsync_SetsGameImageUrl_WhenImageIsUploaded()
    {
        // Arrange
        const string imageBase64 = "base64string";
        const string gameKey = "test-key";
        const string uploadedUrl = "https://cdn/images/test-key.jpg";

        var game = new Game { Key = gameKey };
        
        var gameRequestMock = new Mock<IGameRequest>();
        gameRequestMock.Setup(x => x.Image).Returns(imageBase64);

        var imageServiceMock = new Mock<IGameImageService>();
        imageServiceMock
            .Setup(x => x.UploadImageAsync(gameKey,
                imageBase64, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(uploadedUrl);

        var setupStep = new GameImageSetupStep(imageServiceMock.Object);

        // Act
        await setupStep.InitializeFieldAsync(game, gameRequestMock.Object, CancellationToken.None);

        // Assert
        Assert.Equal(uploadedUrl, game.ImageUrl);
        imageServiceMock.Verify(x =>
                x.UploadImageAsync(gameKey, imageBase64, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}