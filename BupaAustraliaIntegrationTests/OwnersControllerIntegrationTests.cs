using System.Text.Json;
using BupaAustraliaAPI.Interfaces;
using BupaAustraliaAPI.ViewModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BupaAustraliaIntegrationTests;

public class OwnersControllerIntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private Mock<IOwnersService> _ownersServiceMock;
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        _ownersServiceMock = new Mock<IOwnersService>();
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_ownersServiceMock.Object);
                });
            });
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetBooksCategorizedByAge_FilterBooksCorrectly_ForMixedAgeCategories()
    {
        // Arrange
        var categorizedBooks = new List<CategorizedBooks>
        {
            new CategorizedBooks
            {
                AgeCategory = "Child",
                Book = new List<BookDetails>
                {
                    new BookDetails { BookName = "Book1", OwnerName = "Owner1", Age = 10, BookType = "Hardcover" }
                }
            },
            new CategorizedBooks
            {
                AgeCategory = "Adult",
                Book = new List<BookDetails>
                {
                    new BookDetails { BookName = "Book2", OwnerName = "Owner2", Age = 20, BookType = "Paperback" }
                }
            }
        };

        _ownersServiceMock.Setup(service => service.GetBooksCategorizedByAge(true))
                      .ReturnsAsync(categorizedBooks.Where(cb => cb.Book.Any(b => b.BookType == "Hardcover")));

        // Act
        var response = await _client.GetAsync("/api/Owners/booksbycategory?hardcoverOnly=true");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IEnumerable<CategorizedBooks>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Any(c => c.AgeCategory == "Child"), Is.True);
        Assert.That(result.First(c => c.AgeCategory == "Child").Book.Any(b => b.BookType == "Hardcover"), Is.True);
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsNotFound_WhenNoBooksFound()
    {
        // Arrange
        var categorizedBooks = new List<CategorizedBooks>();

        _ownersServiceMock.Setup(service => service.GetBooksCategorizedByAge(true))
                          .ReturnsAsync(categorizedBooks);

        // Act
        var response = await _client.GetAsync("/api/Owners/booksbycategory?hardcoverOnly=true");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsAllBooks_WhenHardcoverOnlyIsFalse()
    {
        // Arrange
        var categorizedBooks = new List<CategorizedBooks>
        {
            new CategorizedBooks
            {
                AgeCategory = "Child",
                Book = new List<BookDetails>
                {
                    new BookDetails { BookName = "Book1", OwnerName = "Owner1", Age = 10, BookType = "Hardcover" },
                    new BookDetails { BookName = "Book2", OwnerName = "Owner1", Age = 10, BookType = "Paperback" }
                }
            },
            new CategorizedBooks
            {
                AgeCategory = "Adult",
                Book = new List<BookDetails>
                {
                    new BookDetails { BookName = "Book3", OwnerName = "Owner2", Age = 20, BookType = "Hardcover" },
                    new BookDetails { BookName = "Book4", OwnerName = "Owner2", Age = 20, BookType = "Paperback" }
                }
            }
        };

        _ownersServiceMock.Setup(service => service.GetBooksCategorizedByAge(false))
                          .ReturnsAsync(categorizedBooks);

        // Act
        var response = await _client.GetAsync("/api/Owners/booksbycategory?hardcoverOnly=false");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IEnumerable<CategorizedBooks>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(c => c.AgeCategory == "Child"), Is.True);
        Assert.That(result.Any(c => c.AgeCategory == "Adult"), Is.True);
        Assert.That(result.First(c => c.AgeCategory == "Child").Book.Count, Is.EqualTo(2));
        Assert.That(result.First(c => c.AgeCategory == "Adult").Book.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsOnlyHardcoverBooks_WhenHardcoverOnlyIsTrue()
    {
        // Arrange
        var categorizedBooks = new List<CategorizedBooks>
        {
            new CategorizedBooks
            {
                AgeCategory = "Child",
                Book = new List<BookDetails>
                {
                    new BookDetails { BookName = "Book1", OwnerName = "Owner1", Age = 10, BookType = "Hardcover" }
                }
            },
            new CategorizedBooks
            {
                AgeCategory = "Adult",
                Book = new List<BookDetails>
                {
                    new BookDetails { BookName = "Book2", OwnerName = "Owner2", Age = 20, BookType = "Hardcover" }
                }
            }
        };

        _ownersServiceMock.Setup(service => service.GetBooksCategorizedByAge(true))
                          .ReturnsAsync(categorizedBooks);

        // Act
        var response = await _client.GetAsync("/api/Owners/booksbycategory?hardcoverOnly=true");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IEnumerable<CategorizedBooks>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(c => c.AgeCategory == "Child"), Is.True);
        Assert.That(result.Any(c => c.AgeCategory == "Adult"), Is.True);
        Assert.That(result.First(c => c.AgeCategory == "Child").Book.Count, Is.EqualTo(1));
        Assert.That(result.First(c => c.AgeCategory == "Adult").Book.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsNotFound_WhenNoHardcoverBooksFound()
    {
        // Arrange
        var categorizedBooks = new List<CategorizedBooks>();

        _ownersServiceMock.Setup(service => service.GetBooksCategorizedByAge(true))
                          .ReturnsAsync(categorizedBooks);

        // Act
        var response = await _client.GetAsync("/api/Owners/booksbycategory?hardcoverOnly=true");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
}