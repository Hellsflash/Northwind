namespace Northwind.Api.DTOs;

public record CustomerDetailDto(
    string CustomerId,
    string CompanyName,
    string? ContactName,
    string? City,
    string? Country,
    IReadOnlyList<OrderSummaryDTO> Orders
);
