using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Options;
using GameStore.BLL.Services.Security;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security;

public class JwtTokenServiceTests
{
    private readonly Mock<IDateTimeProvider> _timeProviderMock;
    private readonly JwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public JwtTokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "supersecretkey1234567890adwadwadawdadsvsdfdsfvsd",
            Issuer = "testissuer",
            Audience = "testaudience",
            ExpirationInMinutes = 60
        };

        var optionsMock = Options.Create(_jwtSettings);

        _timeProviderMock = new Mock<IDateTimeProvider>();
        _timeProviderMock.Setup(tp => tp.UtcNow).Returns(new DateTime(2025, 5, 20, 12, 0, 0, DateTimeKind.Utc));

        _jwtTokenService = new JwtTokenService(optionsMock, _timeProviderMock.Object);
    }

    [Fact]
    public void GenerateAccessToken_ReturnsValidJwtToken()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "testuser"),
            new(ClaimTypes.Role, "Admin")
        };

        // Act
        var tokenString = _jwtTokenService.GenerateAccessToken(claims);

        // Assert
        Assert.False(string.IsNullOrEmpty(tokenString));

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenString);

        Assert.Equal(_jwtSettings.Issuer, token.Issuer);
        Assert.Equal(_jwtSettings.Audience, token.Audiences.Single());

        // Verify claims exist
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Name && c.Value == "testuser");
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");

        // Verify expiration
        var expectedExpiry = _timeProviderMock.Object.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);
        Assert.Equal(expectedExpiry, token.ValidTo, TimeSpan.FromSeconds(1)); // allow 1 sec difference
    }
}