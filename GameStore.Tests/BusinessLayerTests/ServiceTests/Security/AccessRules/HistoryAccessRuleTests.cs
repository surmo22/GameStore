using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.Security.PageAccessRules;
using GameStore.Common.Constants.GameStorePages;
using GameStore.Domain.Entities.Enums;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security.AccessRules;

public class HistoryAccessRuleTests
{
    private readonly Mock<ICurrentUserService> _userServiceMock;
    private readonly HistoryAccessRule _rule;

    public HistoryAccessRuleTests()
    {
        _userServiceMock = new Mock<ICurrentUserService>();
        _rule = new HistoryAccessRule(_userServiceMock.Object);
    }

    [Theory]
    [InlineData(OrderPages.History)]
    public void CanHandle_ValidKey_ReturnsTrue(string key)
    {
        var result = _rule.CanHandle(key);
        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("OtherPage")]
    public void CanHandle_InvalidKey_ReturnsFalse(string? key)
    {
        var result = _rule.CanHandle(key ?? "");
        Assert.False(result);
    }

    [Fact]
    public async Task HasAccessAsync_DelegatesToUserService()
    {
        _userServiceMock
            .Setup(u => u.HasPermissionToAsync(nameof(UserPermissionTypes.ViewOrderHistory),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _rule.HasAccessAsync("anyTargetId", CancellationToken.None);

        Assert.True(result);
        _userServiceMock.Verify(u => u.HasPermissionToAsync(nameof(UserPermissionTypes.ViewOrderHistory),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}