using GameStore.BLL.DTOs.Games;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.GameServices;
using GameStore.BLL.Options;
using Microsoft.Extensions.Options;

namespace GameStore.BLL.Services.GameServices;

public class FilePathService(IOptions<FilePathOptions> options, IDateTimeProvider timeProvider) : IFilePathService
{
    private readonly FilePathOptions _options = options.Value;

    public string GetFilePath(GameDto game)
    {
        var fileName = GetFileName(game);
        var folderPath = Path.Combine(_options.BaseDirectory, _options.BaseFolder);
        EnsureDirectoryExists(folderPath);
        return Path.Combine(folderPath, fileName);
    }

    private string GetFileName(GameDto game)
    {
        var timeStamp = timeProvider.UtcNow.ToString(_options.TimeStampPattern);
        return $"{game.Name}_{timeStamp}.json";
    }

    private static void EnsureDirectoryExists(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }
}