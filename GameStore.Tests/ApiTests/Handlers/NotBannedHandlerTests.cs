using System.Security.Claims;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.WebApi.Authorization.Handlers;
using GameStore.WebApi.Authorization.Requierments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.Handlers;

public class NotBannedHandlerTests
{
    private readonly Mock<IUserBanService> _banServiceMock = new();
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
    public async Task HandleRequirementAsync_UserNotBanned_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _banServiceMock
            .Setup(x => x.IsUserBannedAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var user = CreateUser(userId);
        var requirement = new NotBannedRequirement();
        var authContext = new AuthorizationHandlerContext([requirement], user, null);

        var httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var handler = new NotBannedHandler(_banServiceMock.Object, _httpContextAccessorMock.Object);

        // Act
        await handler.HandleAsync(authContext);

        // Assert
        Assert.True(authContext.HasSucceeded);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserBanned_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _banServiceMock
            .Setup(x => x.IsUserBannedAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var user = CreateUser(userId);
        var requirement = new NotBannedRequirement();
        var authContext = new AuthorizationHandlerContext([requirement], user, null);

        var httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var handler = new NotBannedHandler(_banServiceMock.Object, _httpContextAccessorMock.Object);

        // Act
        await handler.HandleAsync(authContext);

        // Assert
        Assert.False(authContext.HasSucceeded);
    }
}