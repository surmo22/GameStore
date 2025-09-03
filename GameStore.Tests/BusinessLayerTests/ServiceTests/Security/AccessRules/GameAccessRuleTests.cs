using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.Security.PageAccessRules;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security.AccessRules;

public class GameAccessRuleTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IGameRepository> _gameRepoMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly GameAccessRule _rule;

    public GameAccessRuleTests()
    {
        _unitOfWorkMock.Setup(u => u.Games).Returns(_gameRepoMock.Object);
        _rule = new GameAccessRule(_unitOfWorkMock.Object, _currentUserMock.Object);
    }

    [Theory]
    [InlineData("Games")]
    [InlineData("Game")]
    [InlineData("Buy")]
    [InlineData("Comments")]
    [InlineData("Basket")]
    public void CanHandle_ValidKeys_ReturnsTrue(string key)
    {
        var result = _rule.CanHandle(key);
        Assert.True(result);
    }

    [Theory]
    [InlineData("User")]
    [InlineData("")]
    [InlineData(null)]
    public void CanHandle_InvalidKeys_ReturnsFalse(string? key)
    {
        var result = _rule.CanHandle(key ?? "");
        Assert.False(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task HasAccessAsync_NullOrEmptyTargetId_ReturnsTrue(string? targetId)
    {
        var result = await _rule.HasAccessAsync(targetId, CancellationToken.None);
        Assert.True(result);
    }

    [Fact]
    public async Task HasAccessAsync_WithoutViewGamesPermission_ReturnsFalse()
    {
        _currentUserMock.Setup(m => m.HasPermissionToAsync("ViewGames", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _rule.HasAccessAsync("any-key", CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task HasAccessAsync_GameIsNotDeleted_ReturnsTrue()
    {
        _currentUserMock.Setup(m => m.HasPermissionToAsync("ViewGames", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _gameRepoMock.Setup(g => g.GetGameByKeyAsync("key123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Game { IsDeleted = false });

        var result = await _rule.HasAccessAsync("key123", CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task HasAccessAsync_GameIsDeleted_UserHasViewDeletedPermission_ReturnsTrue()
    {
        _currentUserMock.Setup(m => m.HasPermissionToAsync("ViewGames", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _gameRepoMock.Setup(g => g.GetGameByKeyAsync("key123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Game { IsDeleted = true });

        _currentUserMock.Setup(m => m.HasPermissionToAsync("ViewDeletedGames", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _rule.HasAccessAsync("key123", CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task HasAccessAsync_GameIsDeleted_UserLacksViewDeletedPermission_ReturnsFalse()
    {
        _currentUserMock.Setup(m => m.HasPermissionToAsync("ViewGames", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _gameRepoMock.Setup(g => g.GetGameByKeyAsync("key123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Game { IsDeleted = true });

        _currentUserMock.Setup(m => m.HasPermissionToAsync("ViewDeletedGames", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _rule.HasAccessAsync("key123", CancellationToken.None);

        Assert.False(result);
    }
}