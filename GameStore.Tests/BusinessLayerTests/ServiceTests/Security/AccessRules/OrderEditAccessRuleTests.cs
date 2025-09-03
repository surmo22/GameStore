using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.Security.PageAccessRules;
using GameStore.Common.Constants.GameStorePages;
using GameStore.Domain.Entities.Enums;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security.AccessRules;

public class OrderEditAccessRuleTests
{
    private readonly Mock<ICurrentUserService> _userServiceMock;
    private readonly OrderEditAccessRule _rule;

    public OrderEditAccessRuleTests()
    {
        _userServiceMock = new Mock<ICurrentUserService>();
        _rule = new OrderEditAccessRule(_userServiceMock.Object);
    }

    [Theory]
    [InlineData(OrderPages.OrderUpdate)]
    [InlineData(OrderPages.ShipOrder)]
    public void CanHandle_ValidKeys_ReturnsTrue(string key)
    {
        var result = _rule.CanHandle(key);
        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("OtherPage")]
    public void CanHandle_InvalidKeys_ReturnsFalse(string? key)
    {
        var result = _rule.CanHandle(key ?? "");
        Assert.False(result);
    }

    [Fact]
    public async Task HasAccessAsync_DelegatesToUserService_ReturnsTrue()
    {
        _userServiceMock
            .Setup(u => u.HasPermissionToAsync(nameof(UserPermissionTypes.EditOrders), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _rule.HasAccessAsync("anyTargetId", CancellationToken.None);

        Assert.True(result);
        _userServiceMock.Verify(u => u.HasPermissionToAsync(nameof(UserPermissionTypes.EditOrders), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HasAccessAsync_DelegatesToUserService_ReturnsFalse()
    {
        _userServiceMock
            .Setup(u => u.HasPermissionToAsync(nameof(UserPermissionTypes.EditOrders), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _rule.HasAccessAsync("anyTargetId", CancellationToken.None);

        Assert.False(result);
        _userServiceMock.Verify(u => u.HasPermissionToAsync(nameof(UserPermissionTypes.EditOrders), It.IsAny<CancellationToken>()), Times.Once);
    }
}