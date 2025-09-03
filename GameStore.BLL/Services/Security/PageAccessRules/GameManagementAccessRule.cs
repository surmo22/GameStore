using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Interfaces.Security;
using GameStore.Common.Constants.GameStorePages;
using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.Services.Security.PageAccessRules;

public class GameManagementAccessRule(
    ICurrentUserService currentUserService,
    IUnitOfWork         unitOfWork) : IPageAccessRule
{
    private static readonly HashSet<string> Keys = new(StringComparer.OrdinalIgnoreCase)
    {
        GamePages.AddGame,
        GamePages.DeleteGame,
        GamePages.UpdateGame,
    };
    public bool CanHandle(string pageKey)
    {
        return Keys.Contains(pageKey);
    }

    public async Task<bool> HasAccessAsync(string? targetId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(targetId))
        {
            return await currentUserService.HasPermissionToAsync(nameof(UserPermissionTypes.ManageEntities),
                cancellationToken);
        }
        
        var game = await unitOfWork.Games.GetGameByKeyAsync(targetId, cancellationToken);

        if (game.IsDeleted)
        {
            return await currentUserService.HasPermissionToAsync(nameof(UserPermissionTypes.EditDeletedGames),
                cancellationToken);
        }
        
        return await currentUserService.HasPermissionToAsync(nameof(UserPermissionTypes.ManageEntities),
            cancellationToken);
    }
}