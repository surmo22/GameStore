using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.Security.PageAccessRules;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security.AccessRules;

public class UserPanelAccessRuleTests
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly UserPanelAccessRule _rule;

    public UserPanelAccessRuleTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _rule = new UserPanelAccessRule(_currentUserServiceMock.Object);
    }

    [Theory]
    [InlineData("User")]
    [InlineData("Users")]
    public void CanHandle_ValidKeys_ReturnsTrue(string key)
    {
        // Act
        var result = _rule.CanHandle(key);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("Dashboard")]
    [InlineData("Games")]
    [InlineData("Settings")]
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
        _currentUserServiceMock.Setup(x => x.HasPermissionToAsync("ManageUsers", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _rule.HasAccessAsync("someId", CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasAccessAsync_UserLacksPermission_ReturnsFalse()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.HasPermissionToAsync("ManageUsers", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _rule.HasAccessAsync("someId", CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}