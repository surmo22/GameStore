using System.Security.Claims;
using GameStore.BLL.Interfaces.Security;
using GameStore.WebApi.Authorization.Handlers;
using GameStore.WebApi.Authorization.Requierments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.Handlers;

public class PermissionHandlerTests
{
    private readonly Mock<ICurrentUserPermissionService> _permissionServiceMock = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();

    private static ClaimsPrincipal CreateUser(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserHasPermission_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var permission = "SomePermission";

        _permissionServiceMock
            .Setup(p => p.HasPermissionAsync(userId, permission, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var context = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

        var requirement = new PermissionRequirement(permission);
        var user = CreateUser(userId);
        var authContext = new AuthorizationHandlerContext([requirement], user, null);

        var handler = new PermissionHandler(_permissionServiceMock.Object, _httpContextAccessorMock.Object);

        // Act
        await handler.HandleAsync(authContext);

        // Assert
        Assert.True(authContext.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserDoesNotHavePermission_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var permission = "AnotherPermission";

        _permissionServiceMock
            .Setup(p => p.HasPermissionAsync(userId, permission, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var context = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

        var requirement = new PermissionRequirement(permission);
        var user = CreateUser(userId);
        var authContext = new AuthorizationHandlerContext([requirement], user, null);

        var handler = new PermissionHandler(_permissionServiceMock.Object, _httpContextAccessorMock.Object);

        // Act
        await handler.HandleAsync(authContext);

        // Assert
        Assert.False(authContext.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_NoUserClaim_Throws()
    {
        // Arrange
        var permission = "MissingClaim";

        var user = new ClaimsPrincipal(); // No identity
        var requirement = new PermissionRequirement(permission);
        var authContext = new AuthorizationHandlerContext([requirement], user, null);

        var handler = new PermissionHandler(_permissionServiceMock.Object, _httpContextAccessorMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.HandleAsync(authContext));
    }
}