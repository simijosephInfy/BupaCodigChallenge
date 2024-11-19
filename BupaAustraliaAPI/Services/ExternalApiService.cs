using System.Net.Http;
using System.Text.Json;
using BupaAustraliaAPI.Configurations;
using BupaAustraliaAPI.Interfaces;
using BupaAustraliaAPI.Models;
using Microsoft.Extensions.Options;

namespace BupaAustraliaAPI.Services;
public class ExternalApiService(HttpClient httpClient, IOptions<ExternalApiSettings> options) : IExternalApiService
{
    private readonly string endPointUrl = "api/v1/bookowners";
    public async Task<IEnumerable<Owner>> GetBooksCategorizedByAge()
    {
        return await httpClient.GetFromJsonAsync<IEnumerable<Owner>>
            ($"{options.Value.BaseUrl}/{endPointUrl}") ?? Enumerable.Empty<Owner>();
    }
}
