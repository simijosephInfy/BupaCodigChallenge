using BupaAustraliaAPI.Models;

namespace BupaAustraliaAPI.Interfaces;
public interface IExternalApiService
{
    Task<IEnumerable<Owner>> GetBooksCategorizedByAge();
}
