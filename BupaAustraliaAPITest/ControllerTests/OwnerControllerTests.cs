using BupaAustraliaAPI.Controllers;
using BupaAustraliaAPI.Interfaces;
using BupaAustraliaAPI.Services;
using BupaAustraliaAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BupaAustraliaAPITest.ControllerTests;
public class OwnerControllerTests
{
    private Mock<IOwnersService> ownerServiceMock;
    private OwnersController ownerController;

    [SetUp]
    public void SetUp()
    {
        ownerServiceMock = new Mock<IOwnersService>();
        ownerController = new OwnersController(ownerServiceMock.Object);
    }

    [Test]
    public async Task GetCategorizedBooks_ReturnsCategorizedBooks()
    {
        //Arrange
        var categorizedBooks = new List<CategorizedBooks>
        {
        new CategorizedBooks
        {
            AgeCategory ="Child",
            Book = new List<BookDetails>
            {
                new BookDetails { BookName = "Book1", OwnerName = "Owner1", Age = 10 }
            }
        },
        new CategorizedBooks
        {
            AgeCategory ="Adult",
            Book = new List<BookDetails>
            {
                new BookDetails { BookName = "Book2", OwnerName = "Owner1", Age = 20 }
            }
        }
        };
        ownerServiceMock.Setup(service => service.GetBooksCategorizedByAge(false))
            .ReturnsAsync(categorizedBooks);
        //Act
        var result = await ownerController.GetBooksCategorizedByAge();
        //Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(categorizedBooks));
    }
    [Test]
    public async Task GetBooksCategorizedByAge_CategorizesBooksCorrectly_ForMixedAgeCategories()
    {
        // Arrange
        var categorizedBooks = new List<CategorizedBooks>
    {
        new CategorizedBooks
        {
            AgeCategory = "Child",
            Book = new List<BookDetails>
            {
                new BookDetails { BookName = "Book1", OwnerName = "Owner1", Age = 10 }
            }
        },
        new CategorizedBooks
        {
            AgeCategory = "Adult",
            Book = new List<BookDetails>
            {
                new BookDetails { BookName = "Book2", OwnerName = "Owner2", Age = 20 }
            }
        }
    };
        ownerServiceMock.Setup(service => service.GetBooksCategorizedByAge(false))
                        .ReturnsAsync(categorizedBooks);

        // Act
        var result = await ownerController.GetBooksCategorizedByAge();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(categorizedBooks));

        var returnedBooks = okResult.Value as List<CategorizedBooks>;
        Assert.That(returnedBooks, Is.Not.Null);
        Assert.That(returnedBooks.Count, Is.EqualTo(2));

        var childCategory = returnedBooks.FirstOrDefault(c => c.AgeCategory == "Child");
        Assert.That(childCategory, Is.Not.Null);
        Assert.That(childCategory.Book.Count, Is.EqualTo(1));
        Assert.That(childCategory.Book.First().BookName, Is.EqualTo("Book1"));
        Assert.That(childCategory.Book.First().OwnerName, Is.EqualTo("Owner1"));
        Assert.That(childCategory.Book.First().Age, Is.EqualTo(10));

        var adultCategory = returnedBooks.FirstOrDefault(c => c.AgeCategory == "Adult");
        Assert.That(adultCategory, Is.Not.Null);
        Assert.That(adultCategory.Book.Count, Is.EqualTo(1));
        Assert.That(adultCategory.Book.First().BookName, Is.EqualTo("Book2"));
        Assert.That(adultCategory.Book.First().OwnerName, Is.EqualTo("Owner2"));
        Assert.That(adultCategory.Book.First().Age, Is.EqualTo(20));
    }
    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsNotFound_WhenNoData()
    {
        ownerServiceMock.Setup(service => service.GetBooksCategorizedByAge(false))
            .ReturnsAsync(new List<CategorizedBooks>());
        //Act
        var result = await ownerController.GetBooksCategorizedByAge();
        //Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.Value, Is.EqualTo("No categorized books found"));
    }
    [Test]
    public async Task GetBooksCategorizedByAge_ReturnsNotFound_WhenReturnsNull()
    {
        ownerServiceMock.Setup(service => service.GetBooksCategorizedByAge(false))
                          .ReturnsAsync((IEnumerable<CategorizedBooks>)null);
        //Act
        var result = await ownerController.GetBooksCategorizedByAge();
        //Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.Value, Is.EqualTo("No categorized books found"));
    }
    public async Task GetBooksCategorizedByAge_ReturnsNotFound_WhenReturnsEmpty()
    {
        ownerServiceMock.Setup(service => service.GetBooksCategorizedByAge(false))
                          .ReturnsAsync(Enumerable.Empty<CategorizedBooks>);
        //Act
        var result = await ownerController.GetBooksCategorizedByAge();
        //Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.Value, Is.EqualTo("No categorized books found"));
    }

}
