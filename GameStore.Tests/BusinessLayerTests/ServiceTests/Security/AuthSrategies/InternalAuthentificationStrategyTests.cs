using GameStore.BLL.DTOs.User.Login;
using GameStore.BLL.Services.Security.AuthentificationStrategies;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security.AuthSrategies;

public class InternalAuthentificationStrategyTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly InternalAuthentificationStrategy _strategy;

    public InternalAuthentificationStrategyTests()
    {
        _userManagerMock = MockUserManager();
        _strategy = new InternalAuthentificationStrategy(_userManagerMock.Object);
    }

    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsUser()
    {
        // Arrange
        var login = "testuser";
        var password = "password123";
        var user = new User { UserName = login };

        var request = CreateLoginRequest(login, password, internalAuth: true);

        _userManagerMock.Setup(x => x.FindByNameAsync(login))
                        .ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, password))
                        .ReturnsAsync(true);

        // Act
        var result = await _strategy.AuthenticateAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task AuthenticateAsync_UserNotFound_ThrowsUnauthorizedAccess()
    {
        // Arrange
        var request = CreateLoginRequest("nonexistent", "pass", internalAuth: true);

        _userManagerMock.Setup(x => x.FindByNameAsync(request.Model.Login))
                        .ReturnsAsync((User)null!);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _strategy.AuthenticateAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidPassword_ThrowsUnauthorizedAccess()
    {
        // Arrange
        var user = new User { UserName = "test" };
        var request = CreateLoginRequest("test", "wrongpass", internalAuth: true);

        _userManagerMock.Setup(x => x.FindByNameAsync(request.Model.Login))
                        .ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Model.Password))
                        .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _strategy.AuthenticateAsync(request, CancellationToken.None));
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void CanAuthenticate_ReturnsExpected(bool internalAuth, bool expected)
    {
        // Arrange
        var request = CreateLoginRequest("user", "pass", internalAuth);

        // Act
        var result = _strategy.CanAuthenticate(request);

        // Assert
        Assert.Equal(expected, result);
    }

    private static LoginRequestDto CreateLoginRequest(string login, string password, bool internalAuth) =>
        new()
        {
            Model = new LoginModel()
            {
                Login = login,
                Password = password,
                InternalAuth = internalAuth
            }
        };
}