using System.Security.Claims;
using GameStore.BLL.Services.Security;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security;

public class CurrentUserPermissionServiceTests : IDisposable
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<RoleManager<Role>> _roleManagerMock;
    private readonly MemoryCache _memoryCache;
    private readonly CurrentUserPermissionService _service;

    public CurrentUserPermissionServiceTests()
    {
        _userManagerMock = MockUserManager();
        _roleManagerMock = MockRoleManager();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());

        _service = new CurrentUserPermissionService(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _memoryCache);
    }

    [Fact]
    public async Task HasPermissionAsync_ReturnsTrue_WhenUserHasPermission()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, UserName = "testuser" };
        var roles = new List<string> { "Admin" };
        var role = new Role { Name = "Admin" };
        var claims = new List<Claim> { new(ClaimType.Permission, "Permission.Read") };

        _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(roles);
        _roleManagerMock.Setup(r => r.FindByNameAsync("Admin")).ReturnsAsync(role);
        _roleManagerMock.Setup(r => r.GetClaimsAsync(role)).ReturnsAsync(claims);

        // Act
        var result = await _service.HasPermissionAsync(userId, "Permission.Read", CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasPermissionAsync_ReturnsFalse_WhenUserDoesNotHavePermission()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var roles = new List<string> { "User" };
        var role = new Role { Name = "User" };
        var claims = new List<Claim>(); // No permission claims

        _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(roles);
        _roleManagerMock.Setup(r => r.FindByNameAsync("User")).ReturnsAsync(role);
        _roleManagerMock.Setup(r => r.GetClaimsAsync(role)).ReturnsAsync(claims);

        // Act
        var result = await _service.HasPermissionAsync(userId, "Permission.Write", CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasPermissionAsync_UsesCache_IfAvailable()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cacheKey = $"permissions:{userId}";
        var permissions = new List<string> { "Permission.Cached" };
        _memoryCache.Set(cacheKey, permissions, TimeSpan.FromMinutes(5));

        // Act
        var result = await _service.HasPermissionAsync(userId, "Permission.Cached", CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasPermissionAsync_ReturnsFalse_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString())).ReturnsAsync((User)null);

        // Act
        var result = await _service.HasPermissionAsync(userId, "Permission.Any", CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasPermissionAsync_ReturnsFalse_WhenRoleNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var roles = new List<string> { "MissingRole" };

        _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(roles);
        _roleManagerMock.Setup(r => r.FindByNameAsync("MissingRole")).ReturnsAsync((Role)null);

        // Act
        var result = await _service.HasPermissionAsync(userId, "Permission.X", CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    // ---------- Helpers ----------
    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
    }

    private static Mock<RoleManager<Role>> MockRoleManager()
    {
        var store = new Mock<IRoleStore<Role>>();
        return new Mock<RoleManager<Role>>(store.Object, null, null, null, null);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
        GC.SuppressFinalize(this);
    }
}