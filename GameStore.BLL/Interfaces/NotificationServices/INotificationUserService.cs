using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;

namespace GameStore.BLL.Interfaces.NotificationServices;

public interface INotificationUserService
{
    Task<List<User>> GetUsersByClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<UserNotificationTypes>> GetUserNotificationTypesAsync(Guid userId, CancellationToken cancellationToken);
    Task UpdateUserNotificationTypesAsync(Guid userId, List<UserNotificationTypes> notificationTypes, CancellationToken cancellationToken);
}