using GameStore.BLL.Interfaces.NotificationServices;

namespace GameStore.IntegrationTests.Setup.Fakes;

public class FakeAzureServiceBusService : IAzureServiceBusService
{
    public Task SendMessageAsync<T>(T messageObject, string queueName, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}