using System.Security.Claims;
using GameStore.BLL.Interfaces.Security;

namespace GameStore.WebApi.Services;

public class CurrentUserService(
    IHttpContextAccessor httpContextAccessor,
    ICurrentUserPermissionService permissionService)
    : ICurrentUserService
{
    public ClaimsPrincipal User => httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public Guid UserId => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
        ? id 
        : Guid.Empty;

    public Task<bool> HasPermissionToAsync(string permission, CancellationToken cancellationToken)
    {
        return permissionService.HasPermissionAsync(UserId, permission, cancellationToken);
    }
}
