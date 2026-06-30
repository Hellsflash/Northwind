namespace Northwind.Web.Models;

public record CustomerSummaryDTO(
    string CustomerId,
    string CompanyName,
    int OrderCount
);

public record OrderSummaryDTO(
    int OrderId,
    DateTime? OrderDate,
    decimal TotalValue,
    int ProductCount
);

public record CustomerDetailDTO(
    string CustomerId,
    string CompanyName,
    string? ContactName,
    string? City,
    string? Country,
    IReadOnlyList<OrderSummaryDTO> Orders
);
