using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.Interfaces.NotificationServices;

/// <summary>
/// Defines a service interface for managing user notification preferences and handling notifications.
/// </summary>
public interface INotificationService
{
    /// Retrieves the notification types configured for the current user.
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of the current user's configured notification types.</returns>
    Task<List<UserNotificationTypes>> GetUserNotificationTypes(CancellationToken cancellationToken);

    /// Updates the notification type preferences for the current user.
    /// <param name="notificationTypes">A list of notification types to configure for the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateUserNotificationTypes(List<UserNotificationTypes> notificationTypes, CancellationToken cancellationToken);

    /// Sends a notification about the status of an order to users related to the order.
    /// <param name="order">The order whose status change requires notification.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendNotificationAboutOrderStatusToRelatedUsers(Order order, CancellationToken cancellationToken);
}