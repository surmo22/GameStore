using GameStore.BLL.DTOs.Notifications;
using GameStore.BLL.Interfaces.ExternalServices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GameStore.EmailSender;

public class EmailSenderFunction(
    IEmailSender sendGridClient,
    ILogger<EmailSenderFunction> logger)
{
    [Function(nameof(EmailSenderFunction))]
    public async Task Run(
        [ServiceBusTrigger("emailqueue", Connection = "ServiceBusConnection")] EmailPaylod email)
    {
        try
        {
            logger.LogInformation("Function triggered to process a message, sending email.");
            await sendGridClient.SendEmailAsync(email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email.");
        }
    }
}