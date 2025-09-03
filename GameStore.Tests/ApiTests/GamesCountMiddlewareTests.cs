using GameStore.BLL.Interfaces.GameServices;
using GameStore.WebApi.Middlewares;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests;

public class GamesCountMiddlewareTests
{
    private readonly Mock<HttpContext> _httpContextMock = new();
    private readonly Mock<HttpResponse> _httpResponseMock = new();
    private readonly Mock<RequestDelegate> _nextDelegateMock = new();
    private readonly Mock<IGameCountService> _gameCountServiceMock = new();

    [Fact]
    public async Task InvokeAsync_ShouldSetResponseHeaderWithTotalGamesCount()
    {
        // Arrange
        var middleware = new GamesCountMiddleware(_nextDelegateMock.Object, _gameCountServiceMock.Object);
        var cancellationToken = new CancellationToken(false);
        var headersDictionary = new HeaderDictionary();

        _gameCountServiceMock.Setup(service => service.GetTotalGamesCount()).Returns(10);

        _httpContextMock.SetupGet(context => context.RequestAborted).Returns(cancellationToken);
        _httpContextMock.SetupGet(context => context.Response).Returns(_httpResponseMock.Object);
        _httpResponseMock.SetupGet(response => response.Headers).Returns(headersDictionary);
        _httpResponseMock.Setup(response => response.OnStarting(It.IsAny<Func<Task>>())).Callback<Func<Task>>(callback => callback.Invoke());

        // Act
        await middleware.InvokeAsync(_httpContextMock.Object);

        // Assert
        Assert.Contains("x-total-numbers-of-games", headersDictionary.Keys);
        Assert.Equal("10", headersDictionary["x-total-numbers-of-games"]);
        _nextDelegateMock.Verify(next => next.Invoke(_httpContextMock.Object), Times.Once);
    }
}