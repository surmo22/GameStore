using GameStore.BLL.Interfaces.Security;
using GameStore.Common.Constants.GameStorePages;
using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.Services.Security.PageAccessRules;

public class HistoryAccessRule(ICurrentUserService currentUserService) : IPageAccessRule
{
    private static readonly HashSet<string> Keys = new(StringComparer.OrdinalIgnoreCase)
    {
        OrderPages.History,
    };
    
    public bool CanHandle(string pageKey)
    {
        return Keys.Contains(pageKey);
    }

    public async Task<bool> HasAccessAsync(string? targetId, CancellationToken cancellationToken)
    {
        return await currentUserService.HasPermissionToAsync(nameof(UserPermissionTypes.ViewOrderHistory),
            cancellationToken);
    }
}