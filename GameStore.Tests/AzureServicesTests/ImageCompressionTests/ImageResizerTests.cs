using GameStore.ImageCompressor.Services;
using GameStore.ImageCompressor.Utils;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace GameStore.Tests.AzureServicesTests.ImageCompressionTests;

public class ImageResizerTests
{
    private static IOptions<ImageCompressorOptions> CreateOptions(int width = 100, int height = 100)
    {
        return Options.Create(new ImageCompressorOptions
        {
            ImageResizeWidth = width,
            ImageResizeHeight = height
        });
    }

    [Fact]
    public void Resize_ResizesImage_ToMaxDimensions_PreservingAspectRatio()
    {
        // Arrange
        var options = CreateOptions();
        var resizer = new ImageResizer(options);

        using var image = new Image<Rgba32>(200, 50);

        // Act
        resizer.Resize(image);

        // Assert
        Assert.True(image.Width <= 100);
        Assert.True(image.Height <= 100);
        Assert.Equal(100, image.Width);
        Assert.Equal(25, image.Height);
    }
}