using System;
using System.Collections.Generic;
using System.Linq;
using BupaAustraliaAPI.Enums;
using BupaAustraliaAPI.Interfaces;
using BupaAustraliaAPI.Models;
using BupaAustraliaAPI.ViewModels;

namespace BupaAustraliaAPI.Services;
public class OwnersService(IExternalApiService externalApiService) : IOwnersService
{
    public async Task<IEnumerable<CategorizedBooks>> GetBooksCategorizedByAge(bool hardcoverOnly)
    {
        return await CategorizeBookByAge(await externalApiService.GetBooksCategorizedByAge(), hardcoverOnly);
    }
    public Task<IEnumerable<CategorizedBooks>> CategorizeBookByAge(IEnumerable<Owner> owners, bool hardcoverOnly)
    {
        var result = owners?
            .Where(owner => owner.Books != null)
            .SelectMany(owner => owner.Books
            .Where(book => !hardcoverOnly || book.Type == Enum.GetName(typeof(BookType), BookType.Hardcover))
            .Select(book => new
            {
                AgeCategory = owner.Age < 18 ? 
                Enum.GetName(typeof(AgeCategory), AgeCategory.Child) : 
                Enum.GetName(typeof(AgeCategory), AgeCategory.Adult),
                BookDetails = new BookDetails
                {
                    BookName = book.Name,
                    BookType = book.Type,
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

        return Task.FromResult(result ?? Enumerable.Empty<CategorizedBooks>());
    }

}
