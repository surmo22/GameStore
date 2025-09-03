using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.Security.PageAccessRules;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security.AccessRules;

public class CommonEntitiesAccessRuleTests
{
    private readonly Mock<ICurrentUserService> _userServiceMock;
    private readonly CommonEntitiesAccessRule _ruleTests;

    public CommonEntitiesAccessRuleTests()
    {
        _userServiceMock = new Mock<ICurrentUserService>();
        _ruleTests = new CommonEntitiesAccessRule(_userServiceMock.Object);
    }

    [Theory]
    [InlineData("Genre")]
    [InlineData("Genres")]
    [InlineData("Platform")]
    [InlineData("Platforms")]
    [InlineData("Publisher")]
    [InlineData("Publishers")]
    public void CanHandle_ValidKeys_ReturnsTrue(string key)
    {
        var result = _ruleTests.CanHandle(key);
        Assert.True(result);
    }

    [Theory]
    [InlineData("Game")]
    [InlineData("User")]
    [InlineData("")]
    public void CanHandle_InvalidKeys_ReturnsFalse(string key)
    {
        var result = _ruleTests.CanHandle(key);
        Assert.False(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task HasAccessAsync_TargetIdNullOrEmpty_ReturnsTrue(string? targetId)
    {
        var result = await _ruleTests.HasAccessAsync(targetId, CancellationToken.None);
        Assert.True(result);
    }

    [Fact]
    public async Task HasAccessAsync_TargetIdNotEmpty_ReturnsPermissionResult()
    {
        // Arrange
        var targetId = "123";
        _userServiceMock
            .Setup(x => x.HasPermissionToAsync("ViewGames", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _ruleTests.HasAccessAsync(targetId, CancellationToken.None);

        // Assert
        Assert.True(result);
        _userServiceMock.Verify(x => x.HasPermissionToAsync("ViewGames", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HasAccessAsync_TargetIdNotEmpty_ReturnsFalseIfNoPermission()
    {
        // Arrange
        var targetId = "456";
        _userServiceMock
            .Setup(x => x.HasPermissionToAsync("ViewGames", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _ruleTests.HasAccessAsync(targetId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}