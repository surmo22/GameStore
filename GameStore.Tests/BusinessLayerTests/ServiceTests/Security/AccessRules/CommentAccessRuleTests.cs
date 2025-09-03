using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.Security.PageAccessRules;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security.AccessRules;

public class CommentAccessRuleTests
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUserBanService> _userBanServiceMock;
    private readonly CommentAccessRule _rule;

    public CommentAccessRuleTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _userBanServiceMock = new Mock<IUserBanService>();
        _rule = new CommentAccessRule(_currentUserServiceMock.Object, _userBanServiceMock.Object);
    }

    [Theory]
    [InlineData("QuoteComment")]
    [InlineData("ReplyComment")]
    [InlineData("AddComment")]
    public void CanHandle_ValidKeys_ReturnsTrue(string key)
    {
        var result = _rule.CanHandle(key);
        Assert.True(result);
    }

    [Theory]
    [InlineData("PostComment")]
    [InlineData("EditComment")]
    [InlineData("User")]
    [InlineData("")]
    public void CanHandle_InvalidKeys_ReturnsFalse(string key)
    {
        var result = _rule.CanHandle(key);
        Assert.False(result);
    }

    [Fact]
    public async Task HasAccessAsync_UserIsNotBanned_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _userBanServiceMock.Setup(x => x.IsUserBannedAsync(userId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(false);

        // Act
        var result = await _rule.HasAccessAsync(null, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasAccessAsync_UserIsBanned_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _userBanServiceMock.Setup(x => x.IsUserBannedAsync(userId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(true);

        // Act
        var result = await _rule.HasAccessAsync(null, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}