using BupaAustraliaAPI.Interfaces;
using BupaAustraliaAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BupaAustraliaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OwnersController(IOwnersService ownersService) : ControllerBase
{
    [HttpGet("booksbycategory", Name = "GetBooksCategorizedByAge")]
    [SwaggerOperation(Summary = "Gets books categorized by age", Description = "Returns a list of books categorized by the owner's age.")]
    [SwaggerResponse(200, "Returns the list of categorized books", typeof(IEnumerable<CategorizedBooks>))]
    [SwaggerResponse(404, "No categorized books found")]
    public async Task<IActionResult> GetBooksCategorizedByAge()
    {
        var categorizedBooks = await ownersService.GetBooksCategorizedByAge();
        if (categorizedBooks == null || !categorizedBooks.Any())
        {
            return NotFound("No categorized books found");
        }
        return Ok(categorizedBooks);
    }

}
