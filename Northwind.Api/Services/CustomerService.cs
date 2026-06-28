using Microsoft.EntityFrameworkCore;
using Northwind.Api.DTOs;
using Northwind.Api.Interfaces;
using Northwind.Api.Persistence;

namespace Northwind.Api.Services;

public class CustomerService(NorthwindContext context) : ICustomerService
{
    private readonly NorthwindContext _context = context;

    public async Task<CustomerDetailDTO?> GetCustomerDetailAsync(string customerId, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
              .Where(c => c.CustomerId == customerId)
              .Select(c => new CustomerDetailDTO(
                              c.CustomerId,
                              c.CompanyName,
                              c.ContactName,
                              c.City,
                              c.Country,
                              c.Orders
                                 .OrderByDescending(o => o.OrderDate)
                                 .Select(o => new OrderSummaryDTO(
                                     o.OrderId,
                                     o.OrderDate,
                                     Math.Round(
                                         o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)), 2),
                                         o.OrderDetails.Select(od => od.ProductId).Distinct().Count()))
                                 .ToList()))
                                 .FirstOrDefaultAsync(cancellationToken);
        if (customer is null)
        {
            return null;
        }

        return customer;
    }

    public async Task<IReadOnlyList<CustomerSummaryDTO>> GetCustomersAsync(string? search, CancellationToken cancellationToken)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.CompanyName.Contains(search));
        }

        return await query
            .OrderBy(c => c.CompanyName)
            .Select(c => new CustomerSummaryDTO(
                c.CustomerId,
                c.CompanyName,
                c.Orders.Count))
            .ToListAsync(cancellationToken);
    }
}
