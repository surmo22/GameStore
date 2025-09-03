namespace GameStore.BLL.Interfaces.NotificationServices;

/// <summary>
/// Defines the service for interacting with Azure Service Bus.
/// Provides functionality to send messages to specific queues.
/// </summary>
public interface IAzureServiceBusService
{
    /// Sends a message to the specified Azure Service Bus queue.
    /// <typeparam name="T">The type of the message object to be sent.</typeparam>
    /// <param name="messageObject">The object containing the message data to be serialized and sent.</param>
    /// <param name="queueName">The name of the queue to which the message will be sent.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation of sending the message to the queue.</returns>
    Task SendMessageAsync<T>(T messageObject, string queueName, CancellationToken cancellationToken);
}