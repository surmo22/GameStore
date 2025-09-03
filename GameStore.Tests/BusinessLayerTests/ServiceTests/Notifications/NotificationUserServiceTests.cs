using GameStore.BLL.Services.NotificationServices;
using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Notifications;

public class NotificationUserServiceTests
{
    private readonly Mock<RoleManager<Role>> _roleManagerMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly NotificationUserService _service;

    public NotificationUserServiceTests()
    {
        _roleManagerMock = MockRoleManager();
        _userManagerMock = MockUserManager();
        _service = new NotificationUserService(_roleManagerMock.Object, _userManagerMock.Object);
    }

    private static Mock<RoleManager<Role>> MockRoleManager()
    {
        var store = new Mock<IRoleStore<Role>>();
        return new Mock<RoleManager<Role>>(store.Object, null, null, null, null);
    }

    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task GetUsersByClaimAsync_ReturnsDistinctUsers()
    {
        // Arrange
        const string claimType = "permission";
        const string claimValue = "ChangeOrderStatus";
        var cancellationToken = CancellationToken.None;

        var role1 = new Role { Name = "Role1" };
        var role2 = new Role { Name = "Role2" };

        // Set claims on roles
        role1.Claims = new List<IdentityRoleClaim<Guid>> {
            new() { ClaimType = claimType, ClaimValue = claimValue }
        };
        role2.Claims = new List<IdentityRoleClaim<Guid>> {
            new() { ClaimType = claimType, ClaimValue = claimValue }
        };

        var roles = new List<Role> { role1, role2 }.AsQueryable();

        _roleManagerMock.Setup(r => r.Roles).Returns(roles.BuildMock());

        var user1 = new User { Id = Guid.NewGuid(), UserName = "user1" };
        var user2 = new User { Id = Guid.NewGuid(), UserName = "user2" };

        _userManagerMock.Setup(um => um.GetUsersInRoleAsync("Role1"))
            .ReturnsAsync(new List<User> { user1, user2 });
        _userManagerMock.Setup(um => um.GetUsersInRoleAsync("Role2"))
            .ReturnsAsync(new List<User> { user2 }); // user2 appears in both roles, should be distinct

        // Act
        var users = await _service.GetUsersByClaimAsync(claimType, claimValue, cancellationToken);

        // Assert
        Assert.NotNull(users);
        Assert.Equal(2, users.Count); // distinct users: user1 and user2
        Assert.Contains(users, u => u.UserName == "user1");
        Assert.Contains(users, u => u.UserName == "user2");
    }
    
    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, UserName = "testuser" };

        _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task GetUserNotificationTypesAsync_ReturnsNotificationTypes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedTypes = new List<UserNotificationTypes> { UserNotificationTypes.Email, UserNotificationTypes.Push };
        var user = new User { Id = userId, UserNotificationTypes = expectedTypes };

        _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetUserNotificationTypesAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTypes, result);
    }
    
    [Fact]
    public async Task UpdateUserNotificationTypesAsync_UpdatesUserNotificationTypes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newTypes = new List<UserNotificationTypes> { UserNotificationTypes.Sms };
        var user = new User { Id = userId, UserNotificationTypes = new List<UserNotificationTypes>() };

        _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _service.UpdateUserNotificationTypesAsync(userId, newTypes, CancellationToken.None);

        // Assert
        Assert.Equal(newTypes, user.UserNotificationTypes);
        _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
    }

}
