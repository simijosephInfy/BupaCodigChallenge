using BupaAustraliaAPI.Interfaces;
using BupaAustraliaAPI.Models;
using BupaAustraliaAPI.Services;
using Moq;

namespace BupaAustraliaIntegrationTests;

[TestFixture]
public class OwnersServiceIntegrationTests
{
    private Mock<IExternalApiService> _externalApiServiceMock;
    private OwnersService _ownersService;

    [SetUp]
    public void SetUp()
    {
        _externalApiServiceMock = new Mock<IExternalApiService>();
        _ownersService = new OwnersService(_externalApiServiceMock.Object);
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsCategorizedBooks_WhenBooksExist()
    {
        // Arrange
        var owners = new List<Owner>
        {
            new Owner
            {
                Name = "Owner1",
                Age = 10,
                Books = new List<Book>
                {
                    new Book { Name = "Book1", Type = "Hardcover" },
                    new Book { Name = "Book2", Type = "Paperback" }
                }
            },
            new Owner
            {
                Name = "Owner2",
                Age = 20,
                Books = new List<Book>
                {
                    new Book { Name = "Book3", Type = "Hardcover" },
                    new Book { Name = "Book4", Type = "Paperback" }
                }
            }
        };

        _externalApiServiceMock.Setup(service => service.GetBooksCategorizedByAge())
                               .ReturnsAsync(owners);

        // Act
        var result = await _ownersService.GetBooksCategorizedByAge(false);

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
        var owners = new List<Owner>
        {
            new Owner
            {
                Name = "Owner1",
                Age = 10,
                Books = new List<Book>
                {
                    new Book { Name = "Book1", Type = "Hardcover" },
                    new Book { Name = "Book2", Type = "Paperback" }
                }
            },
            new Owner
            {
                Name = "Owner2",
                Age = 20,
                Books = new List<Book>
                {
                    new Book { Name = "Book3", Type = "Hardcover" },
                    new Book { Name = "Book4", Type = "Paperback" }
                }
            }
        };

        _externalApiServiceMock.Setup(service => service.GetBooksCategorizedByAge())
                               .ReturnsAsync(owners);

        // Act
        var result = await _ownersService.GetBooksCategorizedByAge(true);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(c => c.AgeCategory == "Child"), Is.True);
        Assert.That(result.Any(c => c.AgeCategory == "Adult"), Is.True);
        Assert.That(result.First(c => c.AgeCategory == "Child").Book.Count, Is.EqualTo(1));
        Assert.That(result.First(c => c.AgeCategory == "Adult").Book.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsEmpty_WhenNoBooksExist()
    {
        // Arrange
        var owners = new List<Owner>();

        _externalApiServiceMock.Setup(service => service.GetBooksCategorizedByAge())
                               .ReturnsAsync(owners);

        // Act
        var result = await _ownersService.GetBooksCategorizedByAge(false);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsEmpty_WhenNoHardcoverBooksExist()
    {
        // Arrange
        var owners = new List<Owner>
        {
            new Owner
            {
                Name = "Owner1",
                Age = 10,
                Books = new List<Book>
                {
                    new Book { Name = "Book1", Type = "Paperback" }
                }
            },
            new Owner
            {
                Name = "Owner2",
                Age = 20,
                Books = new List<Book>
                {
                    new Book { Name = "Book2", Type = "Paperback" }
                }
            }
        };

        _externalApiServiceMock.Setup(service => service.GetBooksCategorizedByAge())
                               .ReturnsAsync(owners);

        // Act
        var result = await _ownersService.GetBooksCategorizedByAge(true);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
