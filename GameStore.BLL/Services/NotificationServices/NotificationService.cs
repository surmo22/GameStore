using GameStore.BLL.Interfaces.NotificationServices;
using GameStore.BLL.Interfaces.Security;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.Services.NotificationServices;

public class NotificationService(
    ICurrentUserService                currentUserService,
    INotificationUserService           notificationUserService,
    IEnumerable<INotificationStrategy> notificationStrategies) : INotificationService
{
    public async Task<List<UserNotificationTypes>> GetUserNotificationTypes(CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var notificationTypes = await notificationUserService
            .GetUserNotificationTypesAsync(userId, cancellationToken);
        return notificationTypes;
    }

    public async Task UpdateUserNotificationTypes(List<UserNotificationTypes> notificationTypes, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        await notificationUserService.UpdateUserNotificationTypesAsync(userId, notificationTypes, cancellationToken);
    }

    public async Task SendNotificationAboutOrderStatusToRelatedUsers(Order order, CancellationToken cancellationToken)
    {

        var users = await notificationUserService.GetUsersByClaimAsync(
            ClaimType.Permission,
            nameof(UserPermissionTypes.ReceiveNotificationOnOrderStatusChange),
            cancellationToken);
        
        var orderOwner = await notificationUserService.GetUserByIdAsync(order.CustomerId, cancellationToken);
        if (orderOwner != null)
        {
            users.Add(orderOwner);
            users = users.Distinct().ToList();
        }
            
        var tasks = new List<Task>();
        
        foreach (var user in users)
        {
            foreach (var strategy in notificationStrategies)
            {
                if (strategy.IsEnabledForUser(user))
                {
                    tasks.Add(strategy.SendNotificationAsync(
                        user,
                        "Order status changed",
                        $"Order {order.Id} has changed its status to {order.Status.ToString()}",
                        cancellationToken));
                }
            }
        }
        
        await Task.WhenAll(tasks);
    }
}