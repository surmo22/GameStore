using GameStore.BLL.DTOs.Games;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Options;
using GameStore.BLL.Services.GameServices;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests;

public class FilePathServiceTests
{
    [Fact]
    public void GetFilePath_ReturnsCorrectFilePath()
    {
        // Arrange
        var game = new GameDto { Name = "Test Game" };
        var mockOptions = new Mock<IOptions<FilePathOptions>>();
        var timeProvider = new Mock<IDateTimeProvider>();
        var gameFilesSettings = new FilePathOptions
        {
            BaseDirectory = "Games",
            TimeStampPattern = "dd_MM_yyyy",
            BaseFolder = "CustomFolder",
        };

        mockOptions.Setup(o => o.Value).Returns(gameFilesSettings);
        var date = DateTime.UtcNow;
        timeProvider.Setup(t => t.UtcNow).Returns(date);

        var filePathService = new FilePathService(mockOptions.Object, timeProvider.Object);
        var expectedDate = date.ToString("dd_MM_yyyy");
        var expectedFilePath = Path.Combine("Games", "CustomFolder", $"{game.Name}_{expectedDate}.json");

        // Act
        var filePath = filePathService.GetFilePath(game);

        // Assert
        Assert.Equal(expectedFilePath, filePath);
    }

    [Fact]
    public void GetFilePath_ReturnsCorrectFilePath_WhenConfigurationValuesAreNull()
    {
        // Arrange
        var mockOptions = new Mock<IOptions<FilePathOptions>>();
        var gameFilesSettings = new FilePathOptions();

        var timeProvider = new Mock<IDateTimeProvider>();
        var date = DateTime.UtcNow;

        timeProvider.Setup(t => t.UtcNow).Returns(date);
        mockOptions.Setup(o => o.Value).Returns(gameFilesSettings);

        var filePathService = new FilePathService(mockOptions.Object, timeProvider.Object);
        var expectedDate = DateTime.UtcNow.ToString("dd_MM_yyyy");
        var expectedFilePath = Path.Combine(string.Empty, "games", $"Test Game_{expectedDate}.json");

        // Act
        var filePath = filePathService.GetFilePath(new GameDto { Name = "Test Game" });

        // Assert
        Assert.Equal(expectedFilePath, filePath);
    }
}