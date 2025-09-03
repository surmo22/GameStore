using GameStore.BLL.DTOs.User.Login;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.BLL.Services.ExternalServices.Responses;
using GameStore.BLL.Services.Security.AuthentificationStrategies;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security.AuthSrategies;

public class ExternalAuthentificationStrategyTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IAuthentificationExternalService> _externalAuthServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly ExternalAuthentificationStrategy _strategy;

    public ExternalAuthentificationStrategyTests()
    {
        _userManagerMock = MockUserManager();
        _externalAuthServiceMock = new Mock<IAuthentificationExternalService>();
        _userServiceMock = new Mock<IUserService>();

        _strategy = new ExternalAuthentificationStrategy(
            _userManagerMock.Object,
            _externalAuthServiceMock.Object,
            _userServiceMock.Object
        );
    }

    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task AuthenticateAsync_UserExists_ReturnsExistingUser()
    {
        // Arrange
        var loginRequest = CreateLoginRequest("email@example.com", "pass", false);

        var externalResponse = new AuthentificationResponse
        {
            Email = loginRequest.Model.Login,
            FirstName = "John"
        };

        var existingUser = new User { Email = loginRequest.Model.Login };

        _externalAuthServiceMock.Setup(x => x.Authenticate(It.IsAny<AuthentificationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(externalResponse);

        _userManagerMock.Setup(x => x.FindByEmailAsync(externalResponse.Email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _strategy.AuthenticateAsync(loginRequest, CancellationToken.None);

        // Assert
        Assert.Equal(existingUser, result);
        _userServiceMock.Verify(x => x.CreateUserWithoutPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AuthenticateAsync_UserDoesNotExist_CreatesUserAndReturnsIt()
    {
        // Arrange
        var loginRequest = CreateLoginRequest("newuser@example.com", "secret", false);

        var externalResponse = new AuthentificationResponse
        {
            Email = loginRequest.Model.Login,
            FirstName = "Alice"
        };

        var createdUser = new User { Email = externalResponse.Email };

        _externalAuthServiceMock.Setup(x => x.Authenticate(It.IsAny<AuthentificationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(externalResponse);

        _userManagerMock.SetupSequence(x => x.FindByEmailAsync(externalResponse.Email))
            .ReturnsAsync((User)null!)  // first check: not found
            .ReturnsAsync(createdUser); // second check: after creation

        _userServiceMock.Setup(x => x.CreateUserWithoutPasswordAsync(externalResponse.Email, externalResponse.FirstName, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _strategy.AuthenticateAsync(loginRequest, CancellationToken.None);

        // Assert
        Assert.Equal(createdUser, result);
        _userServiceMock.Verify(x => x.CreateUserWithoutPasswordAsync(externalResponse.Email, externalResponse.FirstName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void CanAuthenticate_ReturnsCorrectly(bool internalAuth, bool expected)
    {
        // Arrange
        var loginRequest = CreateLoginRequest("any", "any", internalAuth);

        // Act
        var result = _strategy.CanAuthenticate(loginRequest);

        // Assert
        Assert.Equal(expected, result);
    }

    private static LoginRequestDto CreateLoginRequest(string email, string password, bool internalAuth) =>
        new()
        {
            Model = new LoginModel()
            {
                Login = email,
                Password = password,
                InternalAuth = internalAuth
            }
        };
}