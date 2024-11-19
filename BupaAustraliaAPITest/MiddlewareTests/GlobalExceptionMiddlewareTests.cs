using System.Net;
using System.Text.Json;
using BupaAustraliaAPI.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BupaAustraliaAPITest.MiddlewareTests;
public class GlobalExceptionMiddlewareTests
{
    private DefaultHttpContext context;
    private Mock<RequestDelegate> nextMock;
    private GlobalExceptionMiddleware middleware;

    [SetUp]
    public void Setup()
    {
        context = new DefaultHttpContext();
        nextMock = new Mock<RequestDelegate>();
        middleware = new GlobalExceptionMiddleware();
    }

    [Test]
    public async Task InvokeAsync_WhenNoException_ReturnsSuccess()
    {
        // Arrange
        nextMock.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context, nextMock.Object);

        // Assert
        Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
    }

    [Test]
    public async Task InvokeAsync_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var exception = new Exception("Something went wrong");
        nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await middleware.InvokeAsync(context, nextMock.Object);

        // Assert
        Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
    }
}

