using GameStore.BLL.Attributes;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.Attributes;

public class NotEmptyGuidAttributeTests
{
    private readonly NotEmptyGuidAttribute _attribute;

    public NotEmptyGuidAttributeTests()
    {
        _attribute = new NotEmptyGuidAttribute();
    }

    [Fact]
    public void IsValid_ReturnsTrue_WhenGuidIsNotEmpty()
    {
        var result = _attribute.IsValid(Guid.NewGuid());
        Assert.True(result);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenGuidIsEmpty()
    {
        var result = _attribute.IsValid(Guid.Empty);
        Assert.False(result);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenValueIsNull()
    {
        var result = _attribute.IsValid(null);
        Assert.False(result);
    }

    [Fact]
    public void IsValid_ReturnsFalse_WhenValueIsNotAGuid()
    {
        var result = _attribute.IsValid("not-a-guid");
        Assert.False(result);
    }

    [Fact]
    public void FormatErrorMessage_ReturnsCorrectMessage()
    {
        var result = _attribute.FormatErrorMessage("TestProperty");
        Assert.Equal("TestProperty cannot be an empty GUID.", result);
    }
}