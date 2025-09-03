using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.Security.PageAccessRules;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security.AccessRules;

public class CommentsModerationAccessRuleTests
{
    private readonly Mock<ICurrentUserService> _userServiceMock;
    private readonly CommentsModerationAccessRule _rule;

    public CommentsModerationAccessRuleTests()
    {
        _userServiceMock = new Mock<ICurrentUserService>();
        _rule = new CommentsModerationAccessRule(_userServiceMock.Object);
    }

    [Theory]
    [InlineData("BanComment")]
    [InlineData("DeleteComment")]
    public void CanHandle_ValidKeys_ReturnsTrue(string key)
    {
        // Act
        var result = _rule.CanHandle(key);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("AddComment")]
    [InlineData("QuoteComment")]
    [InlineData("User")]
    [InlineData("")]
    public void CanHandle_InvalidKeys_ReturnsFalse(string key)
    {
        // Act
        var result = _rule.CanHandle(key);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasAccessAsync_UserHasPermission_ReturnsTrue()
    {
        // Arrange
        _userServiceMock
            .Setup(x => x.HasPermissionToAsync("BanUsers", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _rule.HasAccessAsync("irrelevantId", CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasAccessAsync_UserLacksPermission_ReturnsFalse()
    {
        // Arrange
        _userServiceMock
            .Setup(x => x.HasPermissionToAsync("BanUsers", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _rule.HasAccessAsync("irrelevantId", CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}