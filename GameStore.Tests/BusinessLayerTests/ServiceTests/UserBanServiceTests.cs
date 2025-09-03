using GameStore.BLL.DTOs.Comment;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Services.EntityServices;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.Relations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests;

public class UserBanServiceTests : IDisposable
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IDateTimeProvider> _mockTimeProvider;
    private readonly MemoryCache _memoryCache;
    private readonly UserBanService _service;

    public UserBanServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTimeProvider = new Mock<IDateTimeProvider>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        var mockUserService = new Mock<IUserService>();
        var mockLogger = new Mock<ILogger<UserBanService>>();
        _service = new UserBanService(_mockUnitOfWork.Object, _mockTimeProvider.Object, mockUserService.Object, _memoryCache, mockLogger.Object);
    }

    [Fact]
    public async Task IsUserBannedAsync_ShouldCallIsUserBannedAsyncWithCorrectParameters_Once()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentDateTime = DateTime.UtcNow;
        _mockTimeProvider.Setup(tp => tp.UtcNow).Returns(currentDateTime);
        _mockUnitOfWork.Setup(ub => ub.UserBans.IsUserBannedAsync(userId, currentDateTime))
            .ReturnsAsync(true);

        // Act - first call (cache miss, hits DB)
        var result1 = await _service.IsUserBannedAsync(userId, CancellationToken.None);

        // Act - second call (cache hit, no DB call)
        var result2 = await _service.IsUserBannedAsync(userId, CancellationToken.None);

        // Assert
        Assert.True(result1);
        Assert.True(result2);

        // Verify DB call only once despite two service calls
        _mockUnitOfWork.Verify(ub => ub.UserBans.IsUserBannedAsync(userId, currentDateTime), Times.Once);
    }

    [Fact]
    public async Task BanUserAsync_ShouldCreateUserBanAndCallCompleteAsync()
    {
        // Arrange
        var userName = "test";
        var request = new BanUserRequest
        {
            Duration = BanDuration.OneHour,
        };
        var currentDateTime = DateTime.UtcNow;
        _mockTimeProvider.Setup(tp => tp.UtcNow).Returns(currentDateTime);

        var mockUserBans = new Mock<IUserBanRepository>();
        _mockUnitOfWork.Setup(uow => uow.UserBans).Returns(mockUserBans.Object);

        // Act
        await _service.BanUserAsync(userName, request, CancellationToken.None);

        // Assert
        mockUserBans.Verify(ub => ub.AddAsync(It.IsAny<UserBan>(), CancellationToken.None), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task BanUserAsync_ShouldCorrectlyCalculateBannedUntilDateBasedOnDuration()
    {
        // Arrange
        var userName = "Guid.NewGuid()";
        var request = new BanUserRequest
        {
            Duration = BanDuration.OneDay,
        };
        var currentDateTime = DateTime.UtcNow;
        _mockTimeProvider.Setup(tp => tp.UtcNow).Returns(currentDateTime);
        var bannedUntil = currentDateTime.Add(request.Duration.ConvertBanDurationToTimeSpan());

        var mockUserBans = new Mock<IUserBanRepository>();
        _mockUnitOfWork.Setup(uow => uow.UserBans).Returns(mockUserBans.Object);

        // Act
        await _service.BanUserAsync(userName, request, CancellationToken.None);

        // Assert
        Assert.Equal(bannedUntil, currentDateTime.Add(request.Duration.ConvertBanDurationToTimeSpan()));
    }

    [Fact]
    public async Task IsUserBannedAsync_ShouldReturnExpectedResultBasedOnRepositoryCall()
    {
        // Arrange
        var currentDateTime = DateTime.UtcNow;
        var expectedResult = true;
        _mockTimeProvider.Setup(tp => tp.UtcNow).Returns(currentDateTime);
        var mockUserBans = new Mock<IUserBanRepository>();
        mockUserBans.Setup(ub => ub.IsUserBannedAsync(It.IsAny<Guid>(), currentDateTime)).ReturnsAsync(expectedResult);
        _mockUnitOfWork.Setup(uow => uow.UserBans).Returns(mockUserBans.Object);

        // Act
        var result = await _service.IsUserBannedAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
        GC.SuppressFinalize(this);
    }
}