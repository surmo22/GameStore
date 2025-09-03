using GameStore.BLL.Interfaces.GameServices.ImageServices;
using GameStore.BLL.Services.GameServices.ImageServices;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.GameServices.ImageServices;

public class GameImageDataParserTests
{
    private readonly Mock<IImageMimeMapper> _mimeMapperMock;
    private readonly GameImageDataParser _parser;

    public GameImageDataParserTests()
    {
        _mimeMapperMock = new Mock<IImageMimeMapper>();
        _parser = new GameImageDataParser(_mimeMapperMock.Object);
    }

    [Fact]
    public void ParseBase64Image_ValidBase64_ReturnsParsedData()
    {
        // Arrange
        var imageBytes = new byte[] { 1, 2, 3, 4 };
        var base64 = Convert.ToBase64String(imageBytes);
        var mimeType = "image/png";
        var base64Image = $"data:{mimeType};base64,{base64}";

        _mimeMapperMock.Setup(m => m.GetExtension(mimeType)).Returns(".png");

        // Act
        var result = _parser.ParseBase64Image(base64Image);

        // Assert
        Assert.Equal(imageBytes, result.ImageBytes);
        Assert.Equal(mimeType, result.MimeType);
        Assert.Equal(".png", result.Extension);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ParseBase64Image_EmptyString_ThrowsInvalidOperationException(string? input)
    {
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _parser.ParseBase64Image(input!));
        Assert.Equal("Image data is empty.", ex.Message);
    }

    [Fact]
    public void ParseBase64Image_InvalidFormat_ThrowsInvalidOperationException()
    {
        // Arrange
        var invalidBase64 = "this is not base64";

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _parser.ParseBase64Image(invalidBase64));
        Assert.Equal("Invalid image data format.", ex.Message);
    }
}