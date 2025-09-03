using System.Security.Claims;
using GameStore.BLL.DTOs.User.Login;
using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.Security;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security;

public class AuthenticationServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IAuthentificationStrategy> _strategyMock;
    private readonly AuthenticationService _authService;

    public AuthenticationServiceTests()
    {
        _userManagerMock = MockUserManager();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _strategyMock = new Mock<IAuthentificationStrategy>();

        var strategies = new List<IAuthentificationStrategy> { _strategyMock.Object };
        _authService = new AuthenticationService(_userManagerMock.Object, _jwtTokenServiceMock.Object, strategies);
    }

    [Fact]
    public async Task LoginAsync_ValidUser_ReturnsJwtToken()
    {
        // Arrange
        var request = new LoginRequestDto { Model = new() { Login = "null", Password = "null"} };
        var user = new User { Id = Guid.NewGuid(), UserName = "user" };
        var roles = new List<string> { "Admin", "User" };

        _strategyMock.Setup(s => s.CanAuthenticate(request)).Returns(true);
        _strategyMock.Setup(s => s.AuthenticateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(roles);
        _jwtTokenServiceMock.Setup(j => j.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
                            .Returns("mock-token");

        // Act
        var result = await _authService.LoginAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal("Bearer mock-token", result.Token);
        _strategyMock.Verify(s => s.AuthenticateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _userManagerMock.Verify(u => u.GetRolesAsync(user), Times.Once);
        _jwtTokenServiceMock.Verify(j => j.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_StrategyReturnsNullUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequestDto{ Model = new() { Login = "null", Password = "null" }};
        _strategyMock.Setup(s => s.CanAuthenticate(request)).Returns(true);
        _strategyMock.Setup(s => s.AuthenticateAsync(request, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((User)null!);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginAsync(request, CancellationToken.None));
    }

    // Helper to mock UserManager<User>
    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
    }
}