using GameStore.WebApi.Filters.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.Filters;

public class UnauthorizedAccessExceptionHandlerTests
{
    private readonly UnauthorizedAccessExceptionHandler _handler = new();

    [Fact]
    public void CanHandle_ReturnsTrue_ForUnauthorizedAccessException()
    {
        var ex = new UnauthorizedAccessException();

        var canHandle = _handler.CanHandle(ex);

        Assert.True(canHandle);
    }

    [Fact]
    public void CanHandle_ReturnsFalse_ForOtherException()
    {
        var ex = new Exception();

        var canHandle = _handler.CanHandle(ex);

        Assert.False(canHandle);
    }

    [Fact]
    public async Task HandleExceptionAsync_Sets403StatusAndWritesMessage()
    {
        var context = new DefaultHttpContext();
        var logger = Mock.Of<ILogger>();
        var ex = new UnauthorizedAccessException("Access denied");
        var cancellationToken = CancellationToken.None;

        // Prepare response body stream to capture written content
        var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _handler.HandleExceptionAsync(context, ex, logger, cancellationToken);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        responseBody.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(responseBody);
        var responseText = await reader.ReadToEndAsync();

        Assert.Equal(ex.Message, responseText);
    }
}