using GameStore.BLL.Interfaces.Security;
using GameStore.Common.Constants.GameStorePages;
using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.Services.Security.PageAccessRules;

public class CommonEntitiesAccessRule(ICurrentUserService userService) : IPageAccessRule
{
    private static readonly HashSet<string> Keys = new(StringComparer.OrdinalIgnoreCase)
    {
        GenrePages.Genre,
        GenrePages.Genres,
        PlatformPages.Platform,
        PlatformPages.Platforms,
        PublisherPages.Publisher,
        PublisherPages.Publishers,
        NotificationsPages.Notifications
    };
    
    public bool CanHandle(string pageKey)
    {
        return Keys.Contains(pageKey);
    }

    public async Task<bool> HasAccessAsync(string? targetId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(targetId))
        {
            return true;
        }

        return await userService.HasPermissionToAsync(nameof(UserPermissionTypes.ViewGames), cancellationToken);
    }
}