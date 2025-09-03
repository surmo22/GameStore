using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using GameStore.AzureStorage.Services;
using Moq;
using Xunit;

namespace GameStore.Tests.AzureServicesTests.AzureStorageTests;

public class AzureImageStorageServiceTests
{
    [Fact]
    public async Task UploadImageAsync_UploadsCorrectlyAndReturnsUri()
    {
        // Arrange
        var blobClientMock = new Mock<BlobClient>();
        var blobContentInfoMock = new Mock<Response<BlobContentInfo>>();
        blobClientMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(),
                It.IsAny<BlobHttpHeaders>(),
                null,
                null,
                null,
                null,
                default,
                CancellationToken.None))
            .ReturnsAsync(blobContentInfoMock.Object);
        blobClientMock
            .SetupGet(x => x.Uri)
            .Returns(new Uri("https://storage/test/image.jpg"));

        var containerClientMock = new Mock<BlobContainerClient>();
        containerClientMock
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);

        var queueClientMock = new Mock<QueueClient>();

        var service = new AzureImageStorageService(containerClientMock.Object, queueClientMock.Object);

        // Act
        var uri = await service.UploadImageAsync("image.jpg", new byte[10], "image/jpeg", CancellationToken.None);

        // Assert
        Assert.Equal("https://storage/test/image.jpg", uri);
    }
    
    [Fact]
    public async Task DownloadImageAsync_ReturnsCorrectContentAndType()
    {
        // Arrange
        var expectedContent = new byte[] { 1, 2, 3 };
        var expectedContentType = "image/jpeg";

        var downloadResultMock = new Mock<Response<BlobDownloadResult>>();
        downloadResultMock.Setup(x => x.Value).Returns(
            BlobsModelFactory.BlobDownloadResult(
                BinaryData.FromBytes(expectedContent),
                BlobsModelFactory.BlobDownloadDetails(contentType: expectedContentType)
            )
        );

        var blobClientMock = new Mock<BlobClient>();
        blobClientMock
            .Setup(x => x.DownloadContentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(downloadResultMock.Object);

        blobClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

        var containerClientMock = new Mock<BlobContainerClient>();
        containerClientMock
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);

        var queueClientMock = new Mock<QueueClient>();

        var service = new AzureImageStorageService(containerClientMock.Object, queueClientMock.Object);

        // Act
        var (data, contentType) = await service.DownloadImageAsync("image.jpg", CancellationToken.None);

        // Assert
        Assert.Equal(expectedContent, data);
        Assert.Equal(expectedContentType, contentType);
    }
    
    [Fact]
    public async Task DownloadImageAsync_ReturnsEmpty_WhenBlobNotExists()
    {
        // Arrange
        var expectedContent = Array.Empty<byte>() ;
        var expectedContentType = string.Empty;

        var blobClientMock = new Mock<BlobClient>();
        blobClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));

        var containerClientMock = new Mock<BlobContainerClient>();
        containerClientMock
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);

        var queueClientMock = new Mock<QueueClient>();

        var service = new AzureImageStorageService(containerClientMock.Object, queueClientMock.Object);

        // Act
        var (data, contentType) = await service.DownloadImageAsync("image.jpg", CancellationToken.None);

        // Assert
        Assert.Equal(expectedContent, data);
        Assert.Equal(expectedContentType, contentType);
    }
    
    [Fact]
    public async Task DeleteImageAsync_DeletesBlobIfExists()
    {
        // Arrange
        var blobClientMock = new Mock<BlobClient>();
        blobClientMock
            .Setup(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Func<Response<bool>>>());

        var containerClientMock = new Mock<BlobContainerClient>();
        containerClientMock
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);

        var queueClientMock = new Mock<QueueClient>();

        var service = new AzureImageStorageService(containerClientMock.Object, queueClientMock.Object);

        // Act
        await service.DeleteImageAsync("image.jpg", CancellationToken.None);

        // Assert
        blobClientMock.Verify(x => x.DeleteIfExistsAsync(
            It.IsAny<DeleteSnapshotsOption>(), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EnqueueImageProcessingAsync_SendsMessageToQueue()
    {
        // Arrange
        var message = "process-this-image";
        var queueClientMock = new Mock<QueueClient>();
        queueClientMock
            .Setup(x => x.SendMessageAsync(message, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<SendReceipt>>());

        var containerClientMock = new Mock<BlobContainerClient>();

        var service = new AzureImageStorageService(containerClientMock.Object, queueClientMock.Object);

        // Act
        await service.EnqueueImageProcessingAsync(message, CancellationToken.None);

        // Assert
        queueClientMock.Verify(x => x.SendMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

}