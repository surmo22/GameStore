using System.Security.Claims;
using GameStore.BLL.Interfaces.Security;
using GameStore.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests;

public class CurrentUserServiceTests
{
     private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<ICurrentUserPermissionService> _permissionServiceMock;
    private readonly CurrentUserService _currentUserService;

    public CurrentUserServiceTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _permissionServiceMock = new Mock<ICurrentUserPermissionService>();

        _currentUserService = new CurrentUserService(
            _httpContextAccessorMock.Object,
            _permissionServiceMock.Object);
    }

    [Fact]
    public void User_ReturnsUserFromHttpContext()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        }));

        var httpContextMock = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock);

        // Act
        var user = _currentUserService.User;

        // Assert
        Assert.Equal(claimsPrincipal, user);
    }

    [Fact]
    public void User_ReturnsEmptyClaimsPrincipal_WhenHttpContextIsNull()
    {
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        var user = _currentUserService.User;

        Assert.NotNull(user);
        Assert.Empty(user.Claims);
    }

    [Fact]
    public void UserId_ReturnsGuidFromClaims_WhenClaimExistsAndValid()
    {
        var guid = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, guid.ToString())
        }));

        var httpContextMock = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock);

        var userId = _currentUserService.UserId;

        Assert.Equal(guid, userId);
    }

    [Fact]
    public void UserId_ReturnsEmptyGuid_WhenClaimMissingOrInvalid()
    {
        // Case 1: No HttpContext
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        Assert.Equal(Guid.Empty, _currentUserService.UserId);

        // Case 2: HttpContext but no claim
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        var httpContextMock = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock);

        Assert.Equal(Guid.Empty, _currentUserService.UserId);

        // Case 3: HttpContext with invalid Guid claim
        var invalidClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
        }));
        httpContextMock.User = invalidClaimsPrincipal;

        Assert.Equal(Guid.Empty, _currentUserService.UserId);
    }

    [Fact]
    public async Task HasPermissionToAsync_CallsPermissionServiceWithUserIdAndPermission()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var permission = "SomePermission";
        var cancellationToken = CancellationToken.None;

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, guid.ToString())
        }));
        var httpContextMock = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock);

        _permissionServiceMock.Setup(p => p.HasPermissionAsync(guid, permission, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await _currentUserService.HasPermissionToAsync(permission, cancellationToken);

        // Assert
        Assert.True(result);
        _permissionServiceMock.Verify(p => p.HasPermissionAsync(guid, permission, cancellationToken), Times.Once);
    }
}