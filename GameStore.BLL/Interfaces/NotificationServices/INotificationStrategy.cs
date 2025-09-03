using GameStore.Domain.Entities.UserEntities;

namespace GameStore.BLL.Interfaces.NotificationServices;

/// Defines the strategy for sending notifications. Implementations of this interface
/// provide specific mechanisms for delivering notifications to users, such as via email,
/// SMS, or push notifications.
public interface INotificationStrategy
{
    /// <summary>
    /// Sends a notification to the specified user using the defined notification method.
    /// </summary>
    /// <param name="user">The user to whom the notification will be sent.</param>
    /// <param name="subject">The subject or title of the notification.</param>
    /// <param name="message">The message content of the notification.</param>
    /// <param name="cancellationToken">A cancellation token that may be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SendNotificationAsync(User user, string subject, string message,
        CancellationToken cancellationToken);

    /// Determines whether the notification strategy is enabled for the specified user.
    /// <param name="user">The user for whom the notification strategy is being checked.</param>
    /// <returns>True if the notification strategy is enabled for the specified user; otherwise, false.</returns>
    bool IsEnabledForUser(User user);
}