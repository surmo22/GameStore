using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.Security.PageAccessRules;
using GameStore.Common.Constants.GameStorePages;
using GameStore.Domain.Entities.Enums;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security.AccessRules;

public class CommonEntitiesManagementAccessRuleTests
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CommonEntitiesManagementAccessRule _accessRule;

    public CommonEntitiesManagementAccessRuleTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _accessRule = new CommonEntitiesManagementAccessRule(_currentUserServiceMock.Object);
    }

    [Theory]
    [InlineData(GenrePages.AddGenre)]
    [InlineData(GenrePages.DeleteGenre)]
    [InlineData(GenrePages.UpdateGenre)]
    [InlineData(PlatformPages.AddPlatform)]
    [InlineData(PlatformPages.DeletePlatform)]
    [InlineData(PlatformPages.UpdatePlatform)]
    [InlineData(PublisherPages.AddPublisher)]
    [InlineData(PublisherPages.DeletePublisher)]
    [InlineData(PublisherPages.UpdatePublisher)]
    public void CanHandle_ShouldReturnTrue_ForSupportedKeys(string pageKey)
    {
        // Act
        var result = _accessRule.CanHandle(pageKey);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("UnknownPage")]
    [InlineData("AddUser")]
    [InlineData("DeleteGame")]
    public void CanHandle_ShouldReturnFalse_ForUnsupportedKeys(string pageKey)
    {
        // Act
        var result = _accessRule.CanHandle(pageKey);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasAccessAsync_ShouldDelegateToCurrentUserService()
    {
        // Arrange
        var expected = true;
        _currentUserServiceMock
            .Setup(x => x.HasPermissionToAsync(nameof(UserPermissionTypes.ManageEntities), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _accessRule.HasAccessAsync(null, CancellationToken.None);

        // Assert
        Assert.Equal(expected, result);
        _currentUserServiceMock.Verify(
            x => x.HasPermissionToAsync(nameof(UserPermissionTypes.ManageEntities), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}