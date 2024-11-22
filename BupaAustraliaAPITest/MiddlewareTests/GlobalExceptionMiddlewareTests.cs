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

    [Test]
    public async Task InvokeAsync_WhenAggregateExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var innerException = new Exception("Inner exception");
        var aggregateException = new AggregateException("Test aggregate exception", innerException);
        nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(aggregateException);
        var bodyStream = new MemoryStream();
        context.Response.Body = bodyStream;

        // Act
        await middleware.InvokeAsync(context, nextMock.Object);

        // Assert
        Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        bodyStream.Seek(0, System.IO.SeekOrigin.Begin);
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(bodyStream);
        Assert.That(problemDetails.Title, Is.EqualTo("Server Error"));
        Assert.That(problemDetails.Status, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        Assert.That(problemDetails.Detail, Does.StartWith("Test aggregate exception"));
    }
}

