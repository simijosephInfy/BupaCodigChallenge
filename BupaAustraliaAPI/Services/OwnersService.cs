using System.Collections.Generic;
using System.Linq;
using BupaAustraliaAPI.Interfaces;
using BupaAustraliaAPI.Models;
using BupaAustraliaAPI.ViewModels;

namespace BupaAustraliaAPI.Services;
public class OwnersService(IExternalApiService externalApiService) : IOwnersService
{
    public async Task<IEnumerable<CategorizedBooks>> GetBooksCategorizedByAge()
    {
        return await CategorizeBookByAge(await externalApiService.GetBooksCategorizedByAge());
    }
    public Task<IEnumerable<CategorizedBooks>> CategorizeBookByAge(IEnumerable<Owner> owners)
    {
        var result = owners
            .SelectMany(owner => owner.Books.Select(book => new
            {
                AgeCategory = owner.Age < 18 ? "Child" : "Adult",
                BookDetails = new BookDetails
                {
                    BookName = book.Name,
                    OwnerName = owner.Name,
                    Age = owner.Age
                }
            }))
            .GroupBy(x => x.AgeCategory)
            .Select(group => new CategorizedBooks
            {
                AgeCategory = group.Key,
                Book = group.Select(x => x.BookDetails).OrderBy(book => book.BookName).ToList()
            });

        return Task.FromResult(result.AsEnumerable());
    }

}
