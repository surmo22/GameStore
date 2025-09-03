using GameStore.BLL.DTOs.User;
using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.Security;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security;

public class PageAccessCheckerTests
{
    private readonly Mock<IPageAccessRule> _ruleMock;
    private readonly PageAccessChecker _accessChecker;

    public PageAccessCheckerTests()
    {
        _ruleMock = new Mock<IPageAccessRule>();
        var rules = new List<IPageAccessRule> { _ruleMock.Object };
        _accessChecker = new PageAccessChecker(rules);
    }

    [Fact]
    public async Task CheckAccessAsync_RuleHandlesRequest_ReturnsTrue()
    {
        // Arrange
        var request = new CheckAccessRequestDto
        {
            TargetPage = "Games",
            TargetId = "Guid.NewGuid()"
        };

        _ruleMock.Setup(r => r.CanHandle("Games")).Returns(true);
        _ruleMock.Setup(r => r.HasAccessAsync(request.TargetId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

        // Act
        var result = await _accessChecker.CheckAccessAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result);
        _ruleMock.Verify(r => r.HasAccessAsync(request.TargetId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CheckAccessAsync_RuleHandlesRequest_ReturnsFalse()
    {
        // Arrange
        var request = new CheckAccessRequestDto
        {
            TargetPage = "Games",
            TargetId = "Guid.NewGuid()"
        };

        _ruleMock.Setup(r => r.CanHandle("Games")).Returns(true);
        _ruleMock.Setup(r => r.HasAccessAsync(request.TargetId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

        // Act
        var result = await _accessChecker.CheckAccessAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result);
        _ruleMock.Verify(r => r.HasAccessAsync(request.TargetId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CheckAccessAsync_NoRuleCanHandle_ReturnsFalse()
    {
        // Arrange
        var request = new CheckAccessRequestDto
        {
            TargetPage = "UnknownPage",
            TargetId = "Guid.NewGuid()"
        };

        _ruleMock.Setup(r => r.CanHandle("UnknownPage")).Returns(false);

        // Act
        var result = await _accessChecker.CheckAccessAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result);
        _ruleMock.Verify(r => r.HasAccessAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}