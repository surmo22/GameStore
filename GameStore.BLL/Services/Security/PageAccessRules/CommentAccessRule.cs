using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.Security;
using GameStore.Common.Constants.GameStorePages;

namespace GameStore.BLL.Services.Security.PageAccessRules;

public class CommentAccessRule(
    ICurrentUserService currentUserService,
    IUserBanService userBanService) : IPageAccessRule
{
    private static readonly HashSet<string> Keys = new(StringComparer.OrdinalIgnoreCase)
    {
        CommentPages.QuoteComment,
        CommentPages.ReplyComment,
        CommentPages.AddComment,
    };
    
    public bool CanHandle(string pageKey)
    {
        return Keys.Contains(pageKey);
    }

    public async Task<bool> HasAccessAsync(string? targetId, CancellationToken cancellationToken)
    {
        var user = currentUserService.UserId;
        var result = !await userBanService.IsUserBannedAsync(user, cancellationToken);
        return result;
    }
}