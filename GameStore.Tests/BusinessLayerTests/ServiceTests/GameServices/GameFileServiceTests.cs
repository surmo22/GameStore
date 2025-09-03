using GameStore.BLL.DTOs.Games;
using GameStore.BLL.Interfaces.GameServices;
using GameStore.BLL.Services.GameServices;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.GameServices;

public class GameFileServiceTests
{
    private readonly GameFileService _gameFileService;
    private readonly Mock<IFilePathService> _filePathServiceMock;

    public GameFileServiceTests()
    {
        _filePathServiceMock = new Mock<IFilePathService>();
        var loggerMock = new Mock<ILogger<GameFileService>>();
        _gameFileService = new GameFileService(_filePathServiceMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task CreateGameFileAsync_ShouldCreateFileSuccessfully()
    {
        // Arrange
        var game = new GameDto { Name = "Test Game" };
        _filePathServiceMock
            .Setup(s => s.GetFilePath(It.IsAny<GameDto>()))
            .Returns($"games/Test Game_{DateTime.UtcNow:dd_MM_yyyy}.json");
        if (!Directory.Exists("games"))
        {
            Directory.CreateDirectory("games");
        }

        // Act
        var filePath = await _gameFileService.CreateGameFileAsync(game);

        // Assert
        Assert.True(File.Exists(filePath));
        Assert.Contains("Test Game", filePath);
        Assert.Contains(DateTime.UtcNow.ToString("dd_MM_yyyy"), filePath);

        // Clean up
        File.Delete(filePath);
    }

    [Fact]
    public async Task CreateGameFileAsync_ShouldThrowFileNotFoundException_WhenFileWriteFails()
    {
        // Arrange
        var game = new GameDto { Name = "Locked Game" };
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "games");
        var fileName = $"{game.Name}_{DateTime.UtcNow:dd_MM_yyyy}.json";
        var filePath = Path.Combine(folderPath, fileName);

        // Ensure the directory exists with correct permissions
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        else
        {
            var directoryInfo = new DirectoryInfo(folderPath);
            directoryInfo.Attributes &= ~FileAttributes.ReadOnly;
        }

        // Set up mock behavior
        _filePathServiceMock
            .Setup(s => s.GetFilePath(It.IsAny<GameDto>()))
            .Returns(filePath);

        // Write to the file and open it for reading to simulate locked file
        await File.WriteAllTextAsync(filePath, "Initial content");

        // On Linux, check if the file can be opened and locked properly
        await using (var _ = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            // Act & Assert
            await Assert.ThrowsAsync<IOException>(() => _gameFileService.CreateGameFileAsync(game));
        }

        // Clean up
        File.Delete(filePath);
        Directory.Delete(folderPath, true);
    }
}
