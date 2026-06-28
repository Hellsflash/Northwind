namespace Northwind.Api.DTOs;

public record CustomerDetailDTO(
    string CustomerId,
    string CompanyName,
    string? ContactName,
    string? City,
    string? Country,
    IReadOnlyList<OrderSummaryDTO> Orders
);
