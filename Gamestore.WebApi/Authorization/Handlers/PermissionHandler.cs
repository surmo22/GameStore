using System.Security.Claims;
using GameStore.BLL.Interfaces.Security;
using GameStore.WebApi.Authorization.Requierments;
using Microsoft.AspNetCore.Authorization;

namespace GameStore.WebApi.Authorization.Handlers;

public class PermissionHandler(
    ICurrentUserPermissionService permissionService,
    IHttpContextAccessor          httpContextAccessor) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var idClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idClaim == null || !Guid.TryParse(idClaim, out var id))
        {
            context.Fail();
            return;
        }
        
        if (await permissionService.HasPermissionAsync(
                id,
                requirement.Permission,
                httpContextAccessor.HttpContext.RequestAborted))
        {
            context.Succeed(requirement);
            return;
        }
        
        context.Fail();
    }
}