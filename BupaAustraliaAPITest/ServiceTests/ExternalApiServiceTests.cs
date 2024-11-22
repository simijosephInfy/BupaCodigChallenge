using System.Net;
using System.Text.Json;
using BupaAustraliaAPI.Configurations;
using BupaAustraliaAPI.Models;
using BupaAustraliaAPI.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace BupaAustraliaAPITest.ServiceTests;
public class ExternalApiServiceTests
{
    private Mock<HttpMessageHandler> httpMessageHandlerMock;
    private HttpClient httpClient;
    private ExternalApiService externalApiService;
    private IOptions<ExternalApiSettings> options;

    [SetUp]
    public void Setup()
    {
        httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpClient = new HttpClient(httpMessageHandlerMock.Object);
        options = Options.Create(new ExternalApiSettings { BaseUrl = "http://example.com" });
        externalApiService = new ExternalApiService(httpClient, options);
    }
    [TearDown]
    public void TearDown()
    {
        httpClient.Dispose();
    }
    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsOwnersList()
    {
        // Arrange
        var owners = new List<Owner>
    {
        new Owner { Name = "Owner1", Age = 30, Books = new List<Book> { new Book { Name = "Book1", Type = "Type1" } } }
    };
        var jsonResponse = JsonSerializer.Serialize(owners);
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        var result = await externalApiService.GetBooksCategorizedByAge();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Name, Is.EqualTo("Owner1"));
        Assert.That(result.First().Age, Is.EqualTo(30));
        Assert.That(result.First().Books, Has.Count.EqualTo(1));
        Assert.That(result.First().Books.First().Name, Is.EqualTo("Book1"));
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsEmptyList_OnNullResponse()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null")
        };

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        var result = await externalApiService.GetBooksCategorizedByAge();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsEmptyList_OnEmptyJsonResponse()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]")
        };

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        var result = await externalApiService.GetBooksCategorizedByAge();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
    [Test]
    public void GetBooksCategorizedByAge_ThrowsException_OnMalformedJsonResponse()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{ malformed json }")
        };

        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act & Assert
        Assert.That(async () => await externalApiService.GetBooksCategorizedByAge(), Throws.Exception.TypeOf<JsonException>());
    }
    [Test]
    public void GetBooksCategorizedByAge_ThrowsException_OnTimeout()
    {
        // Arrange
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Request timed out"));

        // Act & Assert
        Assert.That(async () => await externalApiService.GetBooksCategorizedByAge(), Throws.Exception.TypeOf<TaskCanceledException>().With.Message.EqualTo("Request timed out"));
    }
    [Test]
    public void GetBooksCategorizedByAge_ThrowsException_OnHttpRequestException()
    {
        // Arrange
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Request failed"));

        // Act & Assert
        Assert.That(async () => await externalApiService.GetBooksCategorizedByAge(), Throws.Exception.TypeOf<HttpRequestException>().With.Message.EqualTo("Request failed"));
    }
}
