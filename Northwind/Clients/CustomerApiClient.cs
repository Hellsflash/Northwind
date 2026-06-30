using System.Net;
using Northwind.Web.Models;

namespace Northwind.Web.Clients;

public class CustomerApiClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IReadOnlyList<CustomerSummaryDTO>> GetCustomersAsync(string? search = null)
    {
        var url = string.IsNullOrWhiteSpace(search)
            ? "/api/customers"
            : $"/api/customers?search={Uri.EscapeDataString(search)}";

        var response = await _httpClient.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return [];
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<CustomerSummaryDTO>>();

        return result ?? [];
    }

    public async Task<CustomerDetailDTO?> GetCustomerDetailAsync(string customerId)
    {
        var response = await _httpClient.GetAsync($"/api/customers/{Uri.EscapeDataString(customerId)}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CustomerDetailDTO>();
    }
}
