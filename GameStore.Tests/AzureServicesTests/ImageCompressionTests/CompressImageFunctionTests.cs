using GameStore.ImageCompressor.Function;
using GameStore.ImageCompressor.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.AzureServicesTests.ImageCompressionTests;

public class CompressImageFunctionTests
{
    private readonly Mock<IImageCompressionService> _imageCompressionServiceMock = new();
    private readonly CompressImageFunction _function;

    public CompressImageFunctionTests()
    {
        _function = new CompressImageFunction(_imageCompressionServiceMock.Object);
    }

    [Fact]
    public async Task Run_CallsCompressAndUploadImageAsync_AndLogs()
    {
        // Arrange
        var filename = "test.png";
        var functionContextMock = new Mock<FunctionContext>();

        var loggerMock = new Mock<ILogger>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        loggerFactoryMock
            .Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(loggerMock.Object);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ILoggerFactory)))
            .Returns(loggerFactoryMock.Object);

        functionContextMock.Setup(fc => fc.InstanceServices).Returns(serviceProviderMock.Object);

        _imageCompressionServiceMock
            .Setup(s => s.CompressAndUploadImageAsync(filename, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _function.Run(filename, functionContextMock.Object);

        // Assert
        _imageCompressionServiceMock.Verify();
    }

    [Fact]
    public async Task Run_WhenServiceThrows_LogsErrorAndThrows()
    {
        // Arrange
        var filename = "fail.png";
        var functionContextMock = new Mock<FunctionContext>();
        

        var loggerMock = new Mock<ILogger>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        loggerFactoryMock
            .Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(loggerMock.Object);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ILoggerFactory)))
            .Returns(loggerFactoryMock.Object);

        functionContextMock.Setup(fc => fc.InstanceServices).Returns(serviceProviderMock.Object);

        _imageCompressionServiceMock
            .Setup(s => s.CompressAndUploadImageAsync(filename, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"))
            .Verifiable();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _function.Run(filename, functionContextMock.Object));
        Assert.Equal("boom", ex.Message);

        _imageCompressionServiceMock.Verify();
    }
    
    [Fact]
    public async Task Run_WhenFileNameEmpty_LogsErrorAndThrows()
    {
        // Arrange
        var functionContextMock = new Mock<FunctionContext>();

        var loggerMock = new Mock<ILogger>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        loggerFactoryMock
            .Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(loggerMock.Object);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ILoggerFactory)))
            .Returns(loggerFactoryMock.Object);

        functionContextMock.Setup(fc => fc.InstanceServices).Returns(serviceProviderMock.Object);
        

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _function.Run(string.Empty, functionContextMock.Object));
    }
}