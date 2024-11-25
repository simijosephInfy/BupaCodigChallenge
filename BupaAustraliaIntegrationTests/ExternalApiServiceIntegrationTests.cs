using System.Net;
using System.Text.Json;
using BupaAustraliaAPI.Configurations;
using BupaAustraliaAPI.Models;
using BupaAustraliaAPI.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace BupaAustraliaIntegrationTests;

[TestFixture]
public class ExternalApiServiceIntegrationTests
{
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private HttpClient _httpClient;
    private IOptions<ExternalApiSettings> _options;
    private ExternalApiService _externalApiService;

    [SetUp]
    public void SetUp()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new System.Uri("https://mocked-api.com")
        };

        var externalApiSettings = new ExternalApiSettings
        {
            BaseUrl = "https://mocked-api.com"
        };
        _options = Options.Create(externalApiSettings);

        _externalApiService = new ExternalApiService(_httpClient, _options);
    }
    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }
    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsOwners_WhenApiCallIsSuccessful()
    {
        // Arrange
        var mockApiResponse = new List<Owner>
            {
                new Owner
                {
                    Name = "John Doe",
                    Age = 30,
                    Books = new List<Book>
                    {
                        new Book { Name = "Book A", Type = "Hardcover" },
                        new Book { Name = "Book B", Type = "Paperback" }
                    }
                },
                new Owner
                {
                    Name = "Jane Smith",
                    Age = 16,
                    Books = new List<Book>
                    {
                        new Book { Name = "Book C", Type = "Hardcover" }
                    }
                }
            };

        var jsonResponse = JsonSerializer.Serialize(mockApiResponse);
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        var result = await _externalApiService.GetBooksCategorizedByAge();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));

        var firstOwner = result.FirstOrDefault();
        Assert.That(firstOwner.Name, Is.EqualTo("John Doe"));
        Assert.That(firstOwner.Age, Is.EqualTo(30));
        Assert.That(firstOwner.Books.Count, Is.EqualTo(2));
        Assert.That(firstOwner.Books.FirstOrDefault().Name, Is.EqualTo("Book A"));
        Assert.That(firstOwner.Books.FirstOrDefault().Type, Is.EqualTo("Hardcover"));
        Assert.That(firstOwner.Books.LastOrDefault().Name, Is.EqualTo("Book B"));
        Assert.That(firstOwner.Books.LastOrDefault().Type, Is.EqualTo("Paperback"));

        var secondOwner = result.LastOrDefault();
        Assert.That(secondOwner.Name, Is.EqualTo("Jane Smith"));
        Assert.That(secondOwner.Age, Is.EqualTo(16));
        Assert.That(secondOwner.Books.Count, Is.EqualTo(1));
        Assert.That(secondOwner.Books.FirstOrDefault().Name, Is.EqualTo("Book C"));
        Assert.That(secondOwner.Books.FirstOrDefault().Type, Is.EqualTo("Hardcover"));
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsEmptyList_WhenApiCallFails()
    {
        // Arrange
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        Assert.That(async () => await _externalApiService.GetBooksCategorizedByAge(), Throws.Exception.TypeOf<HttpRequestException>());
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsEmptyList_WhenApiResponseIsEmpty()
    {
        // Arrange
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]")
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        var result = await _externalApiService.GetBooksCategorizedByAge();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
