using Northwind.Api.DTOs;

namespace Northwind.Api.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerSummaryDTO>> GetCustomersAsync(string? search, CancellationToken cancellationToken = default);

    Task<CustomerDetailDTO?> GetCustomerDetailAsync(string customerId, CancellationToken cancellationToken = default);
}
