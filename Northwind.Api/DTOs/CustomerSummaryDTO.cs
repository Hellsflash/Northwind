namespace Northwind.Api.DTOs;

public record CustomerSummaryDTO(
    string CustomerId,
    string CompanyName,
    int OrderCount
);
