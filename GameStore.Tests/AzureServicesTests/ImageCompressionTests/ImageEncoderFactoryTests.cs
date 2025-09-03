using GameStore.ImageCompressor.Services;
using GameStore.ImageCompressor.Utils;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using Xunit;

namespace GameStore.Tests.AzureServicesTests.ImageCompressionTests;

public class ImageEncoderFactoryTests
{
    private static IOptions<ImageCompressorOptions> CreateOptions(int quality = 75)
    {
        return Options.Create(new ImageCompressorOptions
        {
            ImageQuality = quality
        });
    }

    [Theory]
    [InlineData("JPEG")]
    [InlineData("jpeg")]
    [InlineData("JPG")]
    public void CreateEncoder_ReturnsJpegEncoder_ForJpegFormats(string format)
    {
        // Arrange
        var factory = new ImageEncoderFactory(CreateOptions(90));

        // Act
        var encoder = factory.CreateEncoder(format);

        // Assert
        var jpegEncoder = Assert.IsType<JpegEncoder>(encoder);
        Assert.Equal(90, jpegEncoder.Quality);
    }

    [Fact]
    public void CreateEncoder_ReturnsPngEncoder_ForPng()
    {
        // Arrange
        var factory = new ImageEncoderFactory(CreateOptions());

        // Act
        var encoder = factory.CreateEncoder("PNG");

        // Assert
        Assert.IsType<PngEncoder>(encoder);
    }

    [Fact]
    public void CreateEncoder_ReturnsGifEncoder_ForGif()
    {
        // Arrange
        var factory = new ImageEncoderFactory(CreateOptions());

        // Act
        var encoder = factory.CreateEncoder("GIF");

        // Assert
        Assert.IsType<GifEncoder>(encoder);
    }

    [Fact]
    public void CreateEncoder_ThrowsException_ForUnsupportedFormat()
    {
        // Arrange
        var factory = new ImageEncoderFactory(CreateOptions());

        // Act & Assert
        var ex = Assert.Throws<NotSupportedException>(() => factory.CreateEncoder("BMP"));
        Assert.Equal("Unsupported format: BMP", ex.Message);
    }
}