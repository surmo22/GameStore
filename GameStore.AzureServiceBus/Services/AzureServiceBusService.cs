using System.Text.Json;
using Azure.Messaging.ServiceBus;
using GameStore.BLL.Interfaces.NotificationServices;

namespace GameStore.AzureServiceBus.Services;

public class AzureServiceBusService(ServiceBusClient client) : IAzureServiceBusService
{
    public async Task SendMessageAsync<T>(T messageObject, string queueName, CancellationToken cancellationToken)
    {
        var sender = client.CreateSender(queueName);
        
        var json = JsonSerializer.Serialize(messageObject);
        var message = new ServiceBusMessage(json)
        {
            ContentType = "application/json"
        };

        await sender.SendMessageAsync(message, cancellationToken: cancellationToken);
    }
}