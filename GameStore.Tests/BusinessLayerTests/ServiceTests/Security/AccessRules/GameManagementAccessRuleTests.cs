using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.Security.PageAccessRules;
using GameStore.Common.Constants.GameStorePages;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Enums;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security.AccessRules;

public class GameManagementAccessRuleTests
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GameManagementAccessRule _accessRule;

    public GameManagementAccessRuleTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _accessRule = new GameManagementAccessRule(_currentUserServiceMock.Object, _unitOfWorkMock.Object);
    }

    [Theory]
    [InlineData(GamePages.AddGame)]
    [InlineData(GamePages.DeleteGame)]
    [InlineData(GamePages.UpdateGame)]
    public void CanHandle_ShouldReturnTrue_ForSupportedKeys(string pageKey)
    {
        var result = _accessRule.CanHandle(pageKey);
        Assert.True(result);
    }

    [Theory]
    [InlineData("UnknownPage")]
    [InlineData("BuyGame")]
    [InlineData("EditOrder")]
    public void CanHandle_ShouldReturnFalse_ForUnsupportedKeys(string pageKey)
    {
        var result = _accessRule.CanHandle(pageKey);
        Assert.False(result);
    }

    [Fact]
    public async Task HasAccessAsync_ShouldUseManageEntities_WhenTargetIdIsNull()
    {
        _currentUserServiceMock
            .Setup(x => x.HasPermissionToAsync(nameof(UserPermissionTypes.ManageEntities), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _accessRule.HasAccessAsync(null, CancellationToken.None);

        Assert.True(result);
        _currentUserServiceMock.Verify(
            x => x.HasPermissionToAsync(nameof(UserPermissionTypes.ManageEntities), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HasAccessAsync_ShouldCheckEditDeletedGames_WhenGameIsDeleted()
    {
        var gameKey = "some-key";
        var game = new Game { IsDeleted = true };

        _unitOfWorkMock.Setup(x => x.Games.GetGameByKeyAsync(gameKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);

        _currentUserServiceMock.Setup(x => x.HasPermissionToAsync(nameof(UserPermissionTypes.EditDeletedGames), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _accessRule.HasAccessAsync(gameKey, CancellationToken.None);

        Assert.True(result);
        _currentUserServiceMock.Verify(x =>
            x.HasPermissionToAsync(nameof(UserPermissionTypes.EditDeletedGames), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HasAccessAsync_ShouldCheckManageEntities_WhenGameIsNotDeleted()
    {
        var gameKey = "some-key";
        var game = new Game { IsDeleted = false };

        _unitOfWorkMock.Setup(x => x.Games.GetGameByKeyAsync(gameKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);

        _currentUserServiceMock.Setup(x => x.HasPermissionToAsync(nameof(UserPermissionTypes.ManageEntities), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _accessRule.HasAccessAsync(gameKey, CancellationToken.None);

        Assert.True(result);
        _currentUserServiceMock.Verify(x =>
            x.HasPermissionToAsync(nameof(UserPermissionTypes.ManageEntities), It.IsAny<CancellationToken>()), Times.Once);
    }
}