using GameStore.BLL.Interfaces.NotificationServices;
using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.BLL.Services.NotificationServices;

public class NotificationUserService(RoleManager<Role> roleManager, UserManager<User> userManager)
    : INotificationUserService
{
    public async Task<List<User>> GetUsersByClaimAsync(string claimType, string claimValue,
        CancellationToken cancellationToken)

    {
        var rolesWithClaim = await roleManager.Roles
            .Where(r => r.Claims.Any(c => c.ClaimType == claimType && c.ClaimValue == claimValue))
            .ToListAsync(cancellationToken);

        var users = new List<User>();

        foreach (var role in rolesWithClaim)
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);
            users.AddRange(usersInRole);
        }

        return users.Distinct().ToList();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await userManager.FindByIdAsync(userId.ToString());
    }

    public async Task<List<UserNotificationTypes>> GetUserNotificationTypesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await GetUserByIdAsync(userId, cancellationToken);
        return user.UserNotificationTypes;
    }

    public async Task UpdateUserNotificationTypesAsync(Guid userId, List<UserNotificationTypes> notificationTypes, CancellationToken cancellationToken)
    {
        var user = await GetUserByIdAsync(userId, cancellationToken);
        user.UserNotificationTypes = notificationTypes;
        await userManager.UpdateAsync(user);
    }
}