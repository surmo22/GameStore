using GameStore.BLL.Services.GameServices.ImageServices;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.GameServices.ImageServices;

public class ImageMimeMapperTests
{
    private readonly ImageMimeMapper _mapper = new();

    [Theory]
    [InlineData("image/png", ".png")]
    [InlineData("image/jpeg", ".jpg")]
    [InlineData("image/gif", ".gif")]
    public void GetExtension_KnownMimeTypes_ReturnsCorrectExtension(string mimeType, string expectedExtension)
    {
        // Act
        var result = _mapper.GetExtension(mimeType);

        // Assert
        Assert.Equal(expectedExtension, result);
    }

    [Fact]
    public void GetExtension_MimeTypeIsCaseInsensitive()
    {
        // Arrange
        var mimeType = "IMAGE/PNG";

        // Act
        var result = _mapper.GetExtension(mimeType);

        // Assert
        Assert.Equal(".png", result);
    }

    [Fact]
    public void GetExtension_UnsupportedMimeType_ThrowsException()
    {
        // Arrange
        var unsupportedMimeType = "image/webp";

        // Act & Assert
        var ex = Assert.Throws<NotSupportedException>(() => _mapper.GetExtension(unsupportedMimeType));
        Assert.Equal($"Unsupported image format: {unsupportedMimeType}", ex.Message);
    }
}