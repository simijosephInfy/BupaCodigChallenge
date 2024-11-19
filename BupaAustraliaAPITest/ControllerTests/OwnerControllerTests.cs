using BupaAustraliaAPI.Controllers;
using BupaAustraliaAPI.Interfaces;
using BupaAustraliaAPI.Services;
using BupaAustraliaAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BupaAustraliaAPITest.ControllerTests
{
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
            ownerServiceMock.Setup(service => service.GetBooksCategorizedByAge())
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
        public async Task GetBooksCategorizedByAge_ReturnsNotFound_WhenNoData()
        {
            ownerServiceMock.Setup(service => service.GetBooksCategorizedByAge())
                .ReturnsAsync(new List<CategorizedBooks>());
            //Act
            var result = await ownerController.GetBooksCategorizedByAge();
            //Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.Value, Is.EqualTo("No categorized books found"));
        }

    }
}
