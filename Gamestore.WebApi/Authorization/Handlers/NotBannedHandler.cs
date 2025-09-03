using System.Security.Claims;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.WebApi.Authorization.Requierments;
using Microsoft.AspNetCore.Authorization;

namespace GameStore.WebApi.Authorization.Handlers;

public class NotBannedHandler(
    IUserBanService      userBanService,
    IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<NotBannedRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        NotBannedRequirement        requirement)
    {
        var user = context.User;
        var userIdClaim = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value 
                                     ?? throw new UnauthorizedAccessException());
        if (await userBanService
                .IsUserBannedAsync(userIdClaim, httpContextAccessor.HttpContext.RequestAborted))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}