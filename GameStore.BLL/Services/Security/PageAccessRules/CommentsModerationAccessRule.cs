using GameStore.BLL.Interfaces.Security;
using GameStore.Common.Constants.GameStorePages;
using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.Services.Security.PageAccessRules;

public class CommentsModerationAccessRule(ICurrentUserService userService) : IPageAccessRule
{
    private static readonly HashSet<string> Keys = new(StringComparer.OrdinalIgnoreCase)
    {
        CommentPages.BanComment,
        CommentPages.DeleteComment,
    };
    
    public bool CanHandle(string pageKey)
    {
        return Keys.Contains(pageKey);
    }

    public async Task<bool> HasAccessAsync(string? targetId, CancellationToken cancellationToken)
    {
        return await userService.HasPermissionToAsync(nameof(UserPermissionTypes.BanUsers), cancellationToken);
    }
}