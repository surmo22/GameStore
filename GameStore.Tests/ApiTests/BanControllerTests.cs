using GameStore.BLL.DTOs.Comment;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.Common.Constants;
using GameStore.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests;

public class BanControllerTests
{
    private readonly Mock<IUserBanService> _mockUserBanService;
    private readonly BanController _controller;

    public BanControllerTests()
    {
        _mockUserBanService = new Mock<IUserBanService>();
        _controller = new BanController(_mockUserBanService.Object);
    }

    [Fact]
    public void GetBanDurations_ReturnsExpectedDurations()
    {
        // Arrange
        var expectedDurations = Enum.GetValues(typeof(BanDuration))
            .Cast<BanDuration>()
            .ToList();

        // Act
        var result = _controller.GetBanDurations() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var actualDurations = Assert.IsType<List<BanDuration>>(result.Value);
        Assert.Equal(expectedDurations, actualDurations);
    }

    [Fact]
    public async Task BanUser_ValidRequest_CallsBanUserAsync()
    {
        // Arrange
        var request = new BanUserRequest
        {
            UserName = "testuser",
            Duration = BanDuration.OneDay,
        };
        var cancellationToken = CancellationToken.None;

        _mockUserBanService
            .Setup(service => service.BanUserAsync(request.UserName, request, cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _controller.BanUser(request, cancellationToken);

        // Assert
        Assert.IsType<OkResult>(result);
        _mockUserBanService.Verify();
    }
}