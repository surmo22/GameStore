using GameStore.BLL.DTOs.Notifications;

namespace GameStore.BLL.Interfaces.ExternalServices;

public interface IEmailSender
{
    Task SendEmailAsync(EmailPaylod email, CancellationToken cancellationToken = default);
}