using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data.Data.Seeders;

public static class UserAndRoleSeeder
{
    private static readonly Guid AdminRoleId = Guid.Parse("308660dc-ae51-480f-824d-7dca6714c3e2");
    private static readonly Guid ManagerRoleId = Guid.Parse("54ba416f-6b89-4c53-873d-4fbd48506e6d");
    private static readonly Guid ModeratorRoleId = Guid.Parse("caddad05-120f-48a8-b659-ff4528e5df97");
    private static readonly Guid UserRoleId = Guid.Parse("cf35304b-0241-4b81-8f57-d0dccdccb836");

    public static void SeedUsersAndRolesData(ModelBuilder builder)
    {
        builder.Entity<Role>().HasData(
            new { Id = AdminRoleId,     Name = "Administrator", NormalizedName = "ADMINISTRATOR", IsSystemRole = true },
            new { Id = ManagerRoleId,   Name = "Manager",       NormalizedName = "MANAGER",       IsSystemRole = true },
            new { Id = ModeratorRoleId, Name = "Moderator",     NormalizedName = "MODERATOR",     IsSystemRole = true },
            new { Id = UserRoleId,      Name = "User",          NormalizedName = "USER",          IsSystemRole = true }
        );

        // 2) build a list of IdentityRoleClaim<Guid>
        var roleClaims = new List<IdentityRoleClaim<Guid>>();
        var claimKey = 1;

        // Admin gets all permissions
        foreach (UserPermissionTypes p in Enum.GetValues(typeof(UserPermissionTypes)))
        {
            roleClaims.Add(new IdentityRoleClaim<Guid>
            {
                Id         = claimKey++,
                RoleId     = AdminRoleId,
                ClaimType  = "permission",
                ClaimValue = p.ToString()
            });
        }

        // Manager gets a subset
        var manager = new[]
        {
            UserPermissionTypes.ManageEntities,
            UserPermissionTypes.ViewOrders,
            UserPermissionTypes.EditOrders,
            UserPermissionTypes.ViewOrderHistory,
            UserPermissionTypes.ChangeOrderStatus,
            UserPermissionTypes.ManageComments,
            UserPermissionTypes.BanUsers,
            UserPermissionTypes.ViewGames,
            UserPermissionTypes.AddComments,
            UserPermissionTypes.ReceiveNotificationOnOrderStatusChange,
        };
        
        foreach (var p in manager)
        {
            roleClaims.Add(new IdentityRoleClaim<Guid>
            {
                Id         = claimKey++,
                RoleId     = ManagerRoleId,
                ClaimType  = "permission",
                ClaimValue = p.ToString()
            });
        }

        // Moderator
        var mod = new[]
        {
            UserPermissionTypes.ManageComments,
            UserPermissionTypes.BanUsers,
            UserPermissionTypes.ViewGames,
            UserPermissionTypes.AddComments
        };
        
        foreach (var p in mod)
        {
            roleClaims.Add(new IdentityRoleClaim<Guid>
            {
                Id         = claimKey++,
                RoleId     = ModeratorRoleId,
                ClaimType  = "permission",
                ClaimValue = p.ToString()
            });
        }

        // User
        var usr = new[]
        {
            UserPermissionTypes.ViewGames,
            UserPermissionTypes.AddComments
        };
        
        foreach (var p in usr)
        {
            roleClaims.Add(new IdentityRoleClaim<Guid>
            {
                Id         = claimKey++,
                RoleId     = UserRoleId,
                ClaimType  = "permission",
                ClaimValue = p.ToString()
            });
        }

        // 3) seed the AspNetRoleClaims table
        builder.Entity<IdentityRoleClaim<Guid>>().HasData(roleClaims);
    }
}