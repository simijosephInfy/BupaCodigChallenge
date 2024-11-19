using BupaAustraliaAPI.ViewModels;

namespace BupaAustraliaAPI.Interfaces;
public interface IOwnersService
{
    Task<IEnumerable<CategorizedBooks>> GetBooksCategorizedByAge();
}
