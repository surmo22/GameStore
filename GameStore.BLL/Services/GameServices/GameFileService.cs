using GameStore.BLL.DTOs.Games;
using GameStore.BLL.Interfaces.GameServices;
using Microsoft.Extensions.Logging;

namespace GameStore.BLL.Services.GameServices;

public class GameFileService(IFilePathService filePathService, ILogger<GameFileService> logger) : IGameFileService
{
    public async Task<string> CreateGameFileAsync(GameDto game)
    {
        var gameData = System.Text.Json.JsonSerializer.Serialize(game);
        var filePath = filePathService.GetFilePath(game);

        try
        {
            await File.WriteAllTextAsync(filePath, gameData);
            return filePath;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create game file at {FilePath}", filePath);
            throw new IOException("Failed to create a game file.");
        }
    }
}
