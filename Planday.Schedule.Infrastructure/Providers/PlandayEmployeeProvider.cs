using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Providers;

namespace Planday.Schedule.Infrastructure.Providers;

public class PlandayEmployeeProvider : IPlandayEmployeeProvider
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PlandayEmployeeProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<(string Name, string Email)> GetEmployeeAsync(long id)
    {
        var httpClient = _httpClientFactory.CreateClient("planday");

        var result = await httpClient.GetFromJsonAsync<dynamic>($"/employee/{id}");

        if (result.ValueKind != JsonValueKind.Null)
            return (result.GetProperty("name").GetString(), result.GetProperty("email").GetString());
        return (string.Empty, string.Empty);
    }
}