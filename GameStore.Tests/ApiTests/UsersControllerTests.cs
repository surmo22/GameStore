using GameStore.BLL.DTOs.Notifications;
using GameStore.BLL.DTOs.Role;
using GameStore.BLL.DTOs.User;
using GameStore.BLL.DTOs.User.Creation;
using GameStore.BLL.DTOs.User.Login;
using GameStore.BLL.DTOs.User.Update;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.NotificationServices;
using GameStore.BLL.Interfaces.Security;
using GameStore.Domain.Entities.Enums;
using GameStore.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly Mock<IPageAccessChecker> _pageAccessCheckerMock;
    private readonly Mock<INotificationService> _notificationServiceMock = new();
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _authServiceMock = new Mock<IAuthenticationService>();
        _pageAccessCheckerMock = new Mock<IPageAccessChecker>();

        _controller = new UsersController(
            _userServiceMock.Object,
            _authServiceMock.Object,
            _pageAccessCheckerMock.Object,
            _notificationServiceMock.Object);
    }

    [Fact]
    public async Task GetAccess_ReturnsAccessResult()
    {
        // Arrange
        var request = new CheckAccessRequestDto { TargetPage = "somePage", TargetId = null };
        _pageAccessCheckerMock.Setup(p => p.CheckAccessAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.GetAccess(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.True((bool)okResult.Value!);
    }

    [Fact]
    public async Task Login_ReturnsLoginResponse()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Model = new LoginModel() { Login = "user", Password = "pass" }};
        var loginResponse = new LoginResponseDto { Token = "token" };

        _authServiceMock.Setup(a => a.LoginAsync(loginRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loginResponse);

        // Act
        var result = await _controller.Login(loginRequest, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(loginResponse, okResult.Value);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsUsers()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new() { Id = Guid.NewGuid(), Name = "user1" },
            new() { Id = Guid.NewGuid(), Name = "user2" }
        };
        _userServiceMock.Setup(u => u.GetAllUsers(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAllUsers(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(users, okResult.Value);
    }

    [Fact]
    public async Task GetUserById_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserDto { Id = userId, Name = "user1" };

        _userServiceMock.Setup(u => u.GetUserById(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserById(userId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(user, okResult.Value);
    }

    [Fact]
    public async Task DeleteUser_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(u => u.DeleteUserById(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.DeleteUser(userId, CancellationToken.None);

        Assert.IsType<OkResult>(result);
        _userServiceMock.Verify(u => u.DeleteUserById(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_ReturnsOk()
    {
        var createUserDto = new CreateUserDto
        {
            User = new(){
                Name = "new-user",
            },
            Roles = [Guid.NewGuid()],
            Password = "Password123!"
        };

        _userServiceMock.Setup(u => u.CreateUserAsync(createUserDto, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.CreateUser(createUserDto, CancellationToken.None);

        Assert.IsType<OkResult>(result);
        _userServiceMock.Verify(u => u.CreateUserAsync(createUserDto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOk()
    {
        var updateUserDto = new UpdateUserDto
        {
            User = new(){
                Id = Guid.NewGuid(),
                Name = "new-user",
            },
            Roles = [Guid.NewGuid()],
            Password = "NewPass123!"
        };

        _userServiceMock.Setup(u => u.UpdateUserAsync(updateUserDto, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.UpdateUser(updateUserDto, CancellationToken.None);

        Assert.IsType<OkResult>(result);
        _userServiceMock.Verify(u => u.UpdateUserAsync(updateUserDto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserRoles_ReturnsRoles()
    {
        var userId = Guid.NewGuid();
        var roles = new List<RoleDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Admin" },
            new() { Id = Guid.NewGuid(), Name = "User" }
        };

        _userServiceMock.Setup(u => u.GetUserRolesByUserId(userId, It.IsAny<CancellationToken>())).ReturnsAsync(roles);

        var result = await _controller.GetUserRoles(userId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(roles, okResult.Value);
    }
    
    [Fact]
    public void GetSupportedNotificationTypes_ReturnsAllEnumValues()
    {
        // Act
        var result = _controller.GetSupportedNotificationTypes();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var values = Assert.IsAssignableFrom<List<UserNotificationTypes>>(okResult.Value);
        Assert.Contains(UserNotificationTypes.Email, values);
        Assert.Contains(UserNotificationTypes.Push, values);
    }

    [Fact]
    public async Task GetMyNotifications_ReturnsUserNotifications()
    {
        // Arrange
        var expectedNotifications = new List<UserNotificationTypes>
        {
            UserNotificationTypes.Email
        };

        _notificationServiceMock.Setup(ns => ns.GetUserNotificationTypes(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedNotifications);

        // Act
        var result = await _controller.GetMyNotifications(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var notifications = Assert.IsAssignableFrom<List<UserNotificationTypes>>(okResult.Value);
        Assert.Equal(expectedNotifications, notifications);
    }

    [Fact]
    public async Task UpdateUserNotifications_CallsUpdateAndReturnsOk()
    {
        // Arrange
        var notificationsList = new NotificationsList
        {
            Notifications = [UserNotificationTypes.Email]
        };

        _notificationServiceMock.Setup(ns => ns.UpdateUserNotificationTypes(notificationsList.Notifications, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _controller.UpdateUserNotifications(notificationsList, CancellationToken.None);

        // Assert
        _notificationServiceMock.Verify();
        Assert.IsType<OkResult>(result);
    }
}