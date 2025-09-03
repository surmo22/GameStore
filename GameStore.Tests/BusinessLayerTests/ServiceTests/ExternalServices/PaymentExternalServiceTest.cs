using System.Net;
using System.Text;
using System.Text.Json;
using GameStore.BLL.Services.ExternalServices;
using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.BLL.Services.ExternalServices.Responses;
using Moq;
using Moq.Protected;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.ExternalServices;

public class PaymentExternalServiceTest : IDisposable
{
    private readonly PaymentExternalService _paymentExternalService;
    private readonly HttpClient _httpClient;
    private readonly Mock<HttpMessageHandler> _handlerMock = new();
    
    public PaymentExternalServiceTest()
    {
        _httpClient = new HttpClient(_handlerMock.Object);
        _paymentExternalService = new PaymentExternalService(_httpClient);
    }
    
    [Fact]
    public async Task PayWithBox_ReturnsResultFromExternalService()
    {
        // Arrange
        var requestObject = new BoxPaymentRequest();
        var responseObject = new BoxPaymentResponse();
        var serializedResponseObject = JsonSerializer.Serialize(responseObject);
        const string url = "http://test.com/api/resource";
        
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post && req.RequestUri.ToString() == url && req.Content != null),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(serializedResponseObject, Encoding.UTF8, "application/json"),
            })
            .Verifiable();
        
        // Act
        var result = await _paymentExternalService.PayWithBox(url, requestObject);
        
        // Assert
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task PayWithVisa_ReturnsResultFromExternalService()
    {
        // Arrange
        var requestObject = new VisaPaymentRequest();
        const string url = "http://test.com/api/resource";
        
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post && req.RequestUri.ToString() == url && req.Content != null),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            })
            .Verifiable();
        
        // Act
        await _paymentExternalService.PayWithVisa(url, requestObject);
        
        // Assert
        _handlerMock.Verify();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}