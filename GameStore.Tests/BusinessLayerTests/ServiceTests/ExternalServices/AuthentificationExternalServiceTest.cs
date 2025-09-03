using System.Net;
using System.Text;
using System.Text.Json;
using GameStore.BLL.Services.ExternalServices;
using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.BLL.Services.ExternalServices.Responses;
using JetBrains.Annotations;
using Moq;
using Moq.Protected;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.ExternalServices;

[TestSubject(typeof(AuthentificationExternalService))]
public class AuthentificationExternalServiceTest : IDisposable
{

    private readonly AuthentificationExternalService _paymentExternalService;
    private readonly HttpClient _httpClient;
    private readonly Mock<HttpMessageHandler> _handlerMock = new();
    
    public AuthentificationExternalServiceTest()
    {
        _httpClient = new HttpClient(_handlerMock.Object);
        _httpClient.BaseAddress = new Uri("http://test.com/api/resource");
        _paymentExternalService = new AuthentificationExternalService(_httpClient);
    }
    
    [Fact]
    public async Task Authenticate_SendsRequest()
    {
        // Arrange
        var requestObject = new AuthentificationRequest();
        var responseObject = new AuthentificationResponse();
        var serializedResponseObject = JsonSerializer.Serialize(responseObject);
        
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(serializedResponseObject, Encoding.UTF8, "application/json"),
            })
            .Verifiable();
        
        // Act
        var result = await _paymentExternalService.Authenticate(requestObject, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
    }


    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}