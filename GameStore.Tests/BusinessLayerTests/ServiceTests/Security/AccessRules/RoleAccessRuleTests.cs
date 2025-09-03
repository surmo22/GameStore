using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.Security.PageAccessRules;
using GameStore.Common.Constants.GameStorePages;
using GameStore.Domain.Entities.Enums;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security.AccessRules;

public class RoleAccessRuleTests
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly RoleAccessRule _accessRule;

    public RoleAccessRuleTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _accessRule = new RoleAccessRule(_currentUserServiceMock.Object);
    }

    [Theory]
    [InlineData(RolePages.Roles)]
    [InlineData(RolePages.Role)]
    [InlineData(RolePages.AddRole)]
    [InlineData(RolePages.DeleteRole)]
    [InlineData(RolePages.UpdateRole)]
    public void CanHandle_ShouldReturnTrue_ForSupportedKeys(string pageKey)
    {
        // Act
        var result = _accessRule.CanHandle(pageKey);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("UnknownPage")]
    [InlineData("Games")]
    [InlineData("Users")]
    public void CanHandle_ShouldReturnFalse_ForUnsupportedKeys(string pageKey)
    {
        // Act
        var result = _accessRule.CanHandle(pageKey);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasAccessAsync_ShouldCheckManageRolesPermission()
    {
        // Arrange
        _currentUserServiceMock
            .Setup(x => x.HasPermissionToAsync(nameof(UserPermissionTypes.ManageRoles), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _accessRule.HasAccessAsync(null, CancellationToken.None);

        // Assert
        Assert.True(result);
        _currentUserServiceMock.Verify(
            x => x.HasPermissionToAsync(nameof(UserPermissionTypes.ManageRoles), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}