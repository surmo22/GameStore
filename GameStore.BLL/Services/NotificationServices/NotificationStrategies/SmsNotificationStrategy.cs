using GameStore.BLL.Interfaces.NotificationServices;
using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;

namespace GameStore.BLL.Services.NotificationServices.NotificationStrategies;

public class SmsNotificationStrategy : INotificationStrategy
{
    // Fake for now, task requirement
    public async Task SendNotificationAsync(User user, string subject, string message,
        CancellationToken cancellationToken)
    {
        await Task.Delay(30, cancellationToken);
    }

    public bool IsEnabledForUser(User user)
    {
        return user.UserNotificationTypes.Contains(UserNotificationTypes.Sms);
    }
}