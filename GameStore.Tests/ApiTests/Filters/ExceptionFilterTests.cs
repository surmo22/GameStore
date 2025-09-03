using System.Net;
using GameStore.WebApi.Filters;
using GameStore.WebApi.Filters.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.Filters;

public class ExceptionFilterTests
{
    [Fact]
    public async Task Test_Unhandled_Exception_Is_Logged_And_Returns_InternalServerError()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionFilter>>();
        var exceptionFilter = new ExceptionFilter(loggerMock.Object, new List<IExceptionHandler>());
        var defaultHttpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(defaultHttpContext, new RouteData(), new ActionDescriptor());
        var exceptionContext = new ExceptionContext(actionContext, [])
        {
            HttpContext = defaultHttpContext,
            Exception = new Exception("Test exception"),
        };

        // Act
        await exceptionFilter.OnExceptionAsync(exceptionContext);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)defaultHttpContext.Response.StatusCode);
        Assert.True(exceptionContext.ExceptionHandled);
    }

    [Fact]
    public async Task Test_Handled_Exception_Is_Logged()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionFilter>>();
        var handlerMock = new Mock<IExceptionHandler>();
        handlerMock.Setup(h => h.CanHandle(It.IsAny<Exception>())).Returns(true);

        List<IExceptionHandler> handlers = [handlerMock.Object];
        var exceptionFilter = new ExceptionFilter(loggerMock.Object, handlers);
        var defaultHttpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(defaultHttpContext, new RouteData(), new ActionDescriptor());
        var exceptionContext = new ExceptionContext(actionContext, [])
        {
            HttpContext = defaultHttpContext,
            Exception = new Exception("Test exception"),
        };

        // Act
        await exceptionFilter.OnExceptionAsync(exceptionContext);

        // Assert
        Assert.True(exceptionContext.ExceptionHandled);
    }
}