using System.Text.Json;
using Azure.Messaging.ServiceBus;
using GameStore.AzureServiceBus.Services;
using Moq;
using Xunit;

namespace GameStore.Tests.AzureServicesTests.AzureServiceBusTests;

public class AzureServiceBusServiceTests
{
    private readonly Mock<ServiceBusClient> _mockClient = new();
    private readonly Mock<ServiceBusSender> _mockSender = new();
    private readonly AzureServiceBusService _service;

    public AzureServiceBusServiceTests()
    {
        _mockClient.Setup(c => c.CreateSender(It.IsAny<string>())).Returns(_mockSender.Object);
        _mockSender.Setup(s => s.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _service = new AzureServiceBusService(_mockClient.Object);
    }

    [Fact]
    public async Task SendMessageAsync_SendsMessageWithCorrectContent()
    {
        var testObject = new { Name = "Test", Value = 123 };
        var queueName = "test-queue";
        var cts = new CancellationTokenSource();

        ServiceBusMessage? capturedMessage = null;

        _mockSender.Setup(s => s.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
            .Callback<ServiceBusMessage, CancellationToken>((msg, _) => capturedMessage = msg)
            .Returns(Task.CompletedTask);

        await _service.SendMessageAsync(testObject, queueName, cts.Token);

        _mockClient.Verify(c => c.CreateSender(queueName), Times.Once);
        _mockSender.Verify(s => s.SendMessageAsync(It.IsAny<ServiceBusMessage>(), cts.Token), Times.Once);

        Assert.NotNull(capturedMessage);
        Assert.Equal("application/json", capturedMessage!.ContentType);

        var json = JsonSerializer.Serialize(testObject);
        Assert.Equal(json, capturedMessage.Body.ToString());
    }
}