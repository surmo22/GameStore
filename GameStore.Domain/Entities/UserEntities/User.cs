using GameStore.Domain.Entities.Enums;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Domain.Entities.UserEntities;

public class User : IdentityUser<Guid>
{
    public bool IsActive { get; set; }

    public List<UserNotificationTypes> UserNotificationTypes { get; set; } = [];
}