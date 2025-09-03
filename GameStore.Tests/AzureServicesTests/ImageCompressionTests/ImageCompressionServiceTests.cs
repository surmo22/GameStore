using GameStore.ImageCompressor.Interfaces;
using GameStore.ImageCompressor.Services;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace GameStore.Tests.AzureServicesTests.ImageCompressionTests;

public class ImageCompressionServiceTests
{
    private readonly Mock<IImageStorageService> _storageMock = new();
    private readonly Mock<IImageEncoderFactory> _encoderFactoryMock = new();
    private readonly Mock<IImageResizer> _resizerMock = new();
    private readonly Mock<IImageEncoder> _imageEncoderMock = new();

    private readonly ImageCompressionService _service;

    public ImageCompressionServiceTests()
    {
        _service = new ImageCompressionService(
            _storageMock.Object,
            _encoderFactoryMock.Object,
            _resizerMock.Object
        );
    }

    [Fact]
    public async Task CompressAndUploadImageAsync_CompressesAndUploadsImage()
    {
        // Arrange
        var filename = "test.jpg";
        var cancellationToken = CancellationToken.None;

        // Create a simple 10x10 red test image
        using var testImage = new Image<Rgba32>(10, 10);
        using var testStream = new MemoryStream();
        await testImage.SaveAsJpegAsync(testStream);
        var imageBytes = testStream.ToArray();

        _storageMock.Setup(s => s.DownloadImageAsync(filename, cancellationToken))
            .ReturnsAsync(imageBytes);

        _encoderFactoryMock.Setup(f => f.CreateEncoder("JPEG"))
            .Returns(_imageEncoderMock.Object);
        

        // Act
        await _service.CompressAndUploadImageAsync(filename, cancellationToken);

        // Assert
        _storageMock.Verify(s => s.DownloadImageAsync(filename, cancellationToken), Times.Once);
        _resizerMock.Verify(r => r.Resize(It.IsAny<Image>()), Times.Once);
        _encoderFactoryMock.Verify(f => f.CreateEncoder("JPEG"), Times.Once);
        _storageMock.Verify(s => s.UploadImageAsync(filename, It.IsAny<Stream>(), cancellationToken), Times.Once);
    }
}