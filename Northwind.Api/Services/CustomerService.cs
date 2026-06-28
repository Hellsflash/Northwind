using Northwind.Api.DTOs;
using Northwind.Api.Interfaces;

namespace Northwind.Api.Services;

public class CustomerService : ICustomerService
{
    public async Task<CustomerDetailDto?> GetCustomerDetailAsync(string customerId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public async Task<IReadOnlyList<CustomerSummaryDTO>> GetCustomersAsync(string? search, CancellationToken cancellationToken) => throw new NotImplementedException();
}
