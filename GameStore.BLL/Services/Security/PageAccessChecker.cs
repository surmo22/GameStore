using GameStore.BLL.DTOs.User;
using GameStore.BLL.Interfaces.Security;

namespace GameStore.BLL.Services.Security;

public class PageAccessChecker(IEnumerable<IPageAccessRule> accessRules) : IPageAccessChecker
{
    public async Task<bool> CheckAccessAsync(CheckAccessRequestDto request, CancellationToken cancellationToken)
    {
        var rule = accessRules.FirstOrDefault(r => r.CanHandle(request.TargetPage));
        if (rule == null)
        {
            return false;
        }
        
        return await rule.HasAccessAsync(request.TargetId, cancellationToken);
    }
}