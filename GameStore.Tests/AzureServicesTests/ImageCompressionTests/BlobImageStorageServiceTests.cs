using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GameStore.ImageCompressor.Services;
using GameStore.ImageCompressor.Utils;
using Microsoft.Extensions.Options;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace GameStore.Tests.AzureServicesTests.ImageCompressionTests;

public class BlobImageStorageServiceTests
{
    private readonly Mock<BlobServiceClient> _blobServiceClientMock = new();
    private readonly Mock<BlobContainerClient> _blobContainerClientMock = new();
    private readonly Mock<BlobClient> _blobClientMock = new();
    private readonly BlobImageStorageService _service;

    public BlobImageStorageServiceTests()
    {
        _blobServiceClientMock
            .Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);

        _blobContainerClientMock
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);

        var options = Options.Create(new ImageCompressorOptions
        {
            BlobContainerName = "container",
            ImageResizeWidth = 100,
            ImageResizeHeight = 100,
            ImageQuality = 75
        });

        _service = new BlobImageStorageService(_blobServiceClientMock.Object, options);
    }

    [Fact]
    public async Task UploadImageAsync_UploadsImage()
    {
        // Arrange
        var imageBytes = new byte[100];
        using var stream = new MemoryStream(imageBytes);
        var imageName = "test.png";
        var blobContentInfoMock = new Mock<Response<BlobContentInfo>>();
        _blobClientMock
            .Setup(b => b.UploadAsync(It.IsAny<Stream>(), true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(blobContentInfoMock.Object);
        // Act
        
        await _service.UploadImageAsync(imageName, stream, CancellationToken.None);
        
        // Assert
        _blobClientMock.Verify(b => 
            b.UploadAsync(It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task DownloadImageAsync_ReturnsImage()
    {
        // Arrange
        using var image = new Image<Rgba32>(1, 1);
        using var ms = new MemoryStream();
        await image.SaveAsPngAsync(ms);
        var imageBytes = ms.ToArray();
        
        var blobDownloadContent = BlobsModelFactory.BlobDownloadResult(content: new BinaryData(imageBytes));
        _blobClientMock
            .Setup(b => b.DownloadContentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(blobDownloadContent, null));
        
        // Act
        var result = await _service.DownloadImageAsync("test.png", CancellationToken.None);
        
        // Assert
        _blobClientMock.Verify(b => b.DownloadContentAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(imageBytes, result);
    }
}