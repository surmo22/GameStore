using GameStore.BLL.DTOs.Notifications;
using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GameStore.BLL.Services.ExternalServices;

public class SendGridEmailSender(
    ISendGridClient client,
    IOptions<SenderOptions> options,
    ILogger<SendGridEmailSender> logger)
    : IEmailSender
{
    private readonly SenderOptions _options = options.Value;

    public async Task SendEmailAsync(EmailPaylod email, CancellationToken cancellationToken = default)
    {
        var msg = new SendGridMessage
        {
            From = new EmailAddress(_options.SenderEmail, _options.SenderName),
            Subject = email.Subject,
            PlainTextContent = email.Body,
            HtmlContent = email.Body
        };
        msg.AddTo(new EmailAddress(email.To));

        var response = await client.SendEmailAsync(msg, cancellationToken);

        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            var body = await response.Body.ReadAsStringAsync(cancellationToken);
            logger.LogError("Failed to send email. Status: {Status}, Body: {Body}", response.StatusCode, body);
            throw new InvalidOperationException("Failed to send email.");
        }

        logger.LogInformation("Email sent");
    }
}