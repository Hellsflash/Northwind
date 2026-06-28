using Northwind.Api.DTOs;
using Northwind.Api.Interfaces;

namespace Northwind.Api.Endpoints;

public static class CustomerEndPoints
{
    public static void MapCustomerEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("api/customers").WithTags("Customers");

        group.Map("/", async (string? search, ICustomerService service, CancellationToken ct) =>
        {
            var customers = await service.GetCustomersAsync(search, ct);
            return Results.Ok(customers);
        })
        .WithName("GetCustomers")
        .WithSummary("List customers with order counts, optionally filtered by company name.")
        .Produces<IReadOnlyList<CustomerSummaryDTO>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async (string id, ICustomerService service, CancellationToken ct) =>
        {
            var customer = await service.GetCustomerDetailAsync(id, ct);
            return customer is not null ? Results.Ok(customer) : Results.NotFound();

        })
        .WithName("GetCustomerDetail")
        .WithSummary("Get a single customer's details and order history.")
        .Produces<CustomerDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
