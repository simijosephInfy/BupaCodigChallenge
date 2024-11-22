using BupaAustraliaAPI.Interfaces;
using BupaAustraliaAPI.Models;
using BupaAustraliaAPI.Services;
using Moq;

namespace BupaAustraliaAPITest.ServiceTests;
public class OwnersServiceTests
{
    private Mock<IExternalApiService> _externalApiServiceMock;
    private OwnersService _ownersService;

    [SetUp]
    public void Setup()
    {
        _externalApiServiceMock = new Mock<IExternalApiService>();
        _ownersService = new OwnersService(_externalApiServiceMock.Object);
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsCategorizedBooks()
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
                new Book { Name = "Book1", Type = "Type1" }
            }
        }
    };

        _externalApiServiceMock.Setup(service => service.GetBooksCategorizedByAge())
            .ReturnsAsync(owners);

        // Act
        var result = await _ownersService.GetBooksCategorizedByAge();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ToList(), Has.Count.EqualTo(1));
        Assert.That(result.First().AgeCategory, Is.EqualTo("Child"));
        Assert.That(result.First().Book, Has.Count.EqualTo(1));
        Assert.That(result.First().Book.First().BookName, Is.EqualTo("Book1"));
    }
    [Test]
    public async Task GetBooksCategorizedByAge_CategorizesBooksCorrectly_ForMixedAgeCategories()
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
                    new Book { Name = "Book1", Type = "Type1" }
                }
            },
            new Owner
            {
                Name = "Owner2",
                Age = 20,
                Books = new List<Book>
                {
                    new Book { Name = "Book2", Type = "Type2" }
                }
            }
        };

        _externalApiServiceMock.Setup(service => service.GetBooksCategorizedByAge())
                               .ReturnsAsync(owners);

        // Act
        var result = await _ownersService.GetBooksCategorizedByAge();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ToList(), Has.Count.EqualTo(2));
        Assert.That(result.Any(c => c.AgeCategory == "Child"), Is.True);
        Assert.That(result.Any(c => c.AgeCategory == "Adult"), Is.True);
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsEmptyList_WhenNoOwners()
    {
        // Arrange
        _externalApiServiceMock.Setup(service => service.GetBooksCategorizedByAge())
            .ReturnsAsync(new List<Owner>());

        // Act
        var result = await _ownersService.GetBooksCategorizedByAge();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsEmptyList_WhenOwnerBooksAreEmpty()
    {
        // Arrange
        var owners = new List<Owner>
            {
                new Owner
                {
                    Name = "Owner1",
                    Age = 10,
                    Books = new List<Book>()
                }
            };

        _externalApiServiceMock.Setup(service => service.GetBooksCategorizedByAge())
            .ReturnsAsync(owners);

        // Act
        var result = await _ownersService.GetBooksCategorizedByAge();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsEmptyList_WhenMultipleOwnersHaveNoBooks()
    {
        // Arrange
        var owners = new List<Owner>
            {
                new Owner
                {
                    Name = "Owner1",
                    Age = 10,
                    Books = new List<Book>()
                },
                new Owner
                {
                    Name = "Owner2",
                    Age = 20,
                    Books = new List<Book>()
                }
            };

        _externalApiServiceMock.Setup(service => service.GetBooksCategorizedByAge())
            .ReturnsAsync(owners);

        // Act
        var result = await _ownersService.GetBooksCategorizedByAge();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsEmptyList_WhenOwnerBooksAreNull()
    {
        // Arrange
        var owners = new List<Owner>
    {
        new Owner
        {
            Name = "Owner1",
            Age = 10,
            Books = null
        }
    };

        _externalApiServiceMock.Setup(service => service.GetBooksCategorizedByAge())
                               .ReturnsAsync(owners);

        // Act
        var result = await _ownersService.GetBooksCategorizedByAge();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
    [Test]
    public void GetBooksCategorizedByAge_ThrowsException_WhenExternalApiFails()
    {
        // Arrange
        _externalApiServiceMock.Setup(service => service.GetBooksCategorizedByAge())
            .ThrowsAsync(new System.Exception("External API failure"));

        // Act & Assert
        Assert.ThrowsAsync<System.Exception>(async () => await _ownersService.GetBooksCategorizedByAge());
    }
}
