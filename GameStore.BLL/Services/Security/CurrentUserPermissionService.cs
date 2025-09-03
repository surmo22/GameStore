using GameStore.BLL.Interfaces.Security;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace GameStore.BLL.Services.Security;

public class CurrentUserPermissionService(
    UserManager<User>         userManager,
    RoleManager<Role>         roleManager,
    IMemoryCache              memoryCache)
    : ICurrentUserPermissionService
{
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    private async Task<IList<string>> GetUserPermissionsAsync(
        Guid              id,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return new List<string>();
        }

        var cacheKey = $"permissions:{id}";

        if (memoryCache.TryGetValue(cacheKey, out IList<string> cachedPermissions))
        {
            return cachedPermissions;
        }

        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return new List<string>();
        }

        var roles = await userManager.GetRolesAsync(user);

        var permissionClaims = await GetClaimsByRoles(roles);

        var permissionsList = permissionClaims.ToList();
        memoryCache.Set(cacheKey, permissionsList, _cacheDuration);
        cancellationToken.ThrowIfCancellationRequested();

        return permissionsList;
    }

    private async Task<HashSet<string>> GetClaimsByRoles(IList<string> roles)
    {
        var permissionClaims = new HashSet<string>();
        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                continue;
            }

            var claims = await roleManager.GetClaimsAsync(role);
            foreach (var claim in claims.Where(c => c.Type == ClaimType.Permission))
            {
                permissionClaims.Add(claim.Value);
            }
        }

        return permissionClaims;
    }

    public async Task<bool> HasPermissionAsync(Guid id, string permission,
        CancellationToken cancellationToken)
    {
        var permissions = await GetUserPermissionsAsync(id, cancellationToken);
        return permissions.Contains(permission);
    }
}