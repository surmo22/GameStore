using GameStore.BLL.DTOs.Notifications;
using GameStore.BLL.Interfaces.NotificationServices;
using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.Extensions.Logging;

namespace GameStore.BLL.Services.NotificationServices.NotificationStrategies;

public class EmailNotificationStrategy(
    IAzureServiceBusService            serviceBusService,
    ILogger<EmailNotificationStrategy> logger) : INotificationStrategy
{
    private const string QueueName = "emailqueue";
    
    public async Task SendNotificationAsync(
        User              user,
        string            subject,
        string            message,
        CancellationToken cancellationToken)
    {
        var emailPayload = new EmailPaylod()
        {
            To = user.Email!,
            Body = message,
            Subject = subject
        };

        if (string.IsNullOrEmpty(emailPayload.To))
        {
            logger.LogWarning("Email notification was not sent, because user has no email");
            return;
        }

        await serviceBusService.SendMessageAsync(emailPayload, QueueName, cancellationToken);
        logger.LogInformation("Successfully queued sending email to the user");
    }
    
    public bool IsEnabledForUser(User user)
    {
        return user.UserNotificationTypes.Contains(UserNotificationTypes.Email);
    }
}