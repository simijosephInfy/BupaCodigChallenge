using BupaAustraliaAPI.Interfaces;
using BupaAustraliaAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BupaAustraliaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OwnersController(IOwnersService ownersService) : ControllerBase
{
    [HttpGet("booksbycategory", Name = "GetBooksCategorizedByAge")]
    public async Task<IActionResult> GetBooksCategorizedByAge()
    {
        var categorizedBooks = await ownersService.GetBooksCategorizedByAge();
        if (categorizedBooks == null || !categorizedBooks.Any())
        {
            return NotFound("No categorized books found");
        }
        return Ok(await ownersService.GetBooksCategorizedByAge());
    }

}
