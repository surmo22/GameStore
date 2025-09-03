using GameStore.BLL.Interfaces.Security;
using GameStore.Common.Constants.GameStorePages;
using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.Services.Security.PageAccessRules;

public class CommonEntitiesManagementAccessRule(ICurrentUserService currentUserService) : IPageAccessRule
{
    private static readonly HashSet<string> Keys = new(StringComparer.OrdinalIgnoreCase)
    {
        GenrePages.AddGenre,
        GenrePages.DeleteGenre,
        GenrePages.UpdateGenre,
        PlatformPages.AddPlatform,
        PlatformPages.DeletePlatform,
        PlatformPages.UpdatePlatform,
        PublisherPages.AddPublisher,
        PublisherPages.DeletePublisher,
        PublisherPages.UpdatePublisher,
    };

    public bool CanHandle(string pageKey)
    {
        return Keys.Contains(pageKey);
    }

    public async Task<bool> HasAccessAsync(string? targetId, CancellationToken cancellationToken)
    {
        return await currentUserService.HasPermissionToAsync(nameof(UserPermissionTypes.ManageEntities),
            cancellationToken);
    }
}