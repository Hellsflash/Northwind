using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Api.DTOs;
using Northwind.Api.Persistence;
using Northwind.Api.Persistence.Entities;
using Xunit;

namespace Northwind.Tests;

public class CustomerEndpointsTests : IClassFixture<NorthwindApiFactory>
{
    private readonly NorthwindApiFactory _factory;

    public CustomerEndpointsTests(NorthwindApiFactory factory)
    {
        _factory = factory;
        SeedData();
    }

    private void SeedData()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NorthwindContext>();

        if (context.Customers.Any())
        {
            return;
        }

        context.Customers.Add(new Customer
        {
            CustomerId = "ALFKI",
            CompanyName = "Alfreds Futterkiste",
            ContactName = "Maria Anders",
            City = "Berlin",
            Country = "Germany"
        });

        context.SaveChanges();
    }

    [Fact]
    public async Task GetCustomers_ReturnsSeededCustomer()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/customers", TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var customers = await response.Content.ReadFromJsonAsync<List<CustomerSummaryDTO>>(cancellationToken: TestContext.Current.CancellationToken);
        customers.Should().Contain(c => c.CustomerId == "ALFKI");
    }

    [Fact]
    public async Task GetCustomerDetail_WithKnownId_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/customers/ALFKI", TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var detail = await response.Content.ReadFromJsonAsync<CustomerDetailDTO>(cancellationToken: TestContext.Current.CancellationToken);
        detail!.CompanyName.Should().Be("Alfreds Futterkiste");
    }

    [Fact]
    public async Task GetCustomerDetail_WithUnknownId_ReturnsNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/customers/ZZZZZ", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
