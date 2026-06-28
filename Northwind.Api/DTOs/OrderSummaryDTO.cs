namespace Northwind.Api.DTOs;

public record OrderSummaryDTO(
    int OrderId,
    DateTime? OrderDate,
    decimal TotalValue,
    int ProductCount
);
