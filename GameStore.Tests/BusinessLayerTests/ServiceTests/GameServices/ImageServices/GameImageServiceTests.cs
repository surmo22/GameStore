using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Interfaces.GameServices.ImageServices;
using GameStore.BLL.Services.GameServices.ImageServices;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.GameServices.ImageServices;

public class GameImageServiceTests
{
    private readonly Mock<IImageDataParser> _dataParserMock = new();
    private readonly Mock<IImageStorageService> _storageMock = new();
    private readonly GameImageService _service;

    public GameImageServiceTests()
    {
        _service = new GameImageService(_dataParserMock.Object, _storageMock.Object);
    }

    [Fact]
    public async Task UploadImageAsync_ValidImage_UploadsAndEnqueues_AndReturnsFilename()
    {
        // Arrange
        var gameKey = "game123";
        var base64 = "data:image/png;base64,iVBORw0KGgoAAAANS...";

        var imageBytes = new byte[] { 1, 2, 3 };
        var mimeType = "image/png";
        var extension = ".png";
        var filename = $"{gameKey}{extension}";

        _dataParserMock.Setup(p => p.ParseBase64Image(base64))
            .Returns((imageBytes, mimeType, extension));

        // Act
        var result = await _service.UploadImageAsync(gameKey, base64, CancellationToken.None);

        // Assert
        Assert.Equal(filename, result);

        _storageMock.Verify(s => s.UploadImageAsync(filename, imageBytes, mimeType, It.IsAny<CancellationToken>()), Times.Once);
        _storageMock.Verify(s => s.EnqueueImageProcessingAsync(filename, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task UploadImageAsync_EmptyImageData_ReturnsEmptyString(string? imageData)
    {
        // Act
        var result = await _service.UploadImageAsync("gameKey", imageData!, CancellationToken.None);

        // Assert
        Assert.Equal(string.Empty, result);
        _dataParserMock.Verify(p => p.ParseBase64Image(It.IsAny<string>()), Times.Never);
        _storageMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DeleteImageAsync_EmptyFilename_DoesNothing(string? filename)
    {
        // Act
        await _service.DeleteImageAsync(filename!, CancellationToken.None);

        // Assert
        _storageMock.Verify(s => s.DeleteImageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteImageAsync_ValidFilename_CallsDelete()
    {
        // Arrange
        var filename = "game123.png";

        // Act
        await _service.DeleteImageAsync(filename, CancellationToken.None);

        // Assert
        _storageMock.Verify(s => s.DeleteImageAsync(filename, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetImage_EmptyFilename_ReturnsEmptyResult()
    {
        // Act
        var result = await _service.GetImage("", CancellationToken.None);

        // Assert
        Assert.Equal([], result.Data);
        Assert.Equal(string.Empty, result.ContentType);
        _storageMock.Verify(s => s.DownloadImageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetImage_ValidFilename_ReturnsData()
    {
        // Arrange
        var filename = "game123.png";
        var data = new byte[] { 5, 6, 7 };
        var contentType = "image/png";

        _storageMock.Setup(s => s.DownloadImageAsync(filename, It.IsAny<CancellationToken>()))
            .ReturnsAsync((data, contentType));

        // Act
        var result = await _service.GetImage(filename, CancellationToken.None);

        // Assert
        Assert.Equal(data, result.Data);
        Assert.Equal(contentType, result.ContentType);
    }
}