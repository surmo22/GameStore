using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Interfaces.Security;
using GameStore.Common.Constants.GameStorePages;
using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.Services.Security.PageAccessRules;

public class GameAccessRule(
    IUnitOfWork         unitOfWork,
    ICurrentUserService currentUserService) : IPageAccessRule
{
    private static readonly HashSet<string> Keys = new(StringComparer.OrdinalIgnoreCase)
    {
        GamePages.Games,
        GamePages.Game,
        GamePages.Buy,
        GamePages.Comments,
        GamePages.Basket,
        GamePages.MakeOrder,
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

        if (!await currentUserService.HasPermissionToAsync(nameof(UserPermissionTypes.ViewGames), cancellationToken))
        {
            return false;
        }
        
        var game = await unitOfWork.Games.GetGameByKeyAsync(targetId, cancellationToken);
        if (!game.IsDeleted)
        {
            return true;
        }

        return await currentUserService.HasPermissionToAsync(nameof(UserPermissionTypes.ViewDeletedGames),
            cancellationToken);
    }
}