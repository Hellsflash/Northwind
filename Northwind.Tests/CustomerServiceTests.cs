using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Northwind.Api.Persistence;
using Northwind.Api.Persistence.Entities;
using Northwind.Api.Services;
using Xunit;

namespace Northwind.Tests;

public class CustomerServiceTests : IDisposable
{
    private readonly NorthwindContext _context;

    public CustomerServiceTests()
    {
        _context = CreateContext(Guid.NewGuid().ToString());
        SeedCustomerWithOrders(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    private static NorthwindContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<NorthwindContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new NorthwindContext(options);
    }

    private static void SeedCustomerWithOrders(NorthwindContext context)
    {
        var customer = new Customer
        {
            CustomerId = "ALFKI",
            CompanyName = "Alfreds Futterkiste",
            ContactName = "Maria Anders",
            City = "Berlin",
            Country = "Germany"
        };

        var order = new Order
        {
            OrderId = 10001,
            CustomerId = "ALFKI",
            OrderDate = new DateTime(2024, 1, 15)
        };

        order.OrderDetails.Add(new OrderDetail { OrderId = 10001, ProductId = 1, UnitPrice = 10m, Quantity = 5, Discount = 0f });
        order.OrderDetails.Add(new OrderDetail { OrderId = 10001, ProductId = 2, UnitPrice = 20m, Quantity = 2, Discount = 0.1f });

        customer.Orders.Add(order);
        context.Customers.Add(customer);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetCustomersAsync_ReturnsCustomerWithCorrectOrderCount()
    {
        var service = new CustomerService(_context);

        var result = await service.GetCustomersAsync(search: null, TestContext.Current.CancellationToken);

        result.Should().ContainSingle();
        result[0].CustomerId.Should().Be("ALFKI");
        result[0].OrderCount.Should().Be(1);
    }

    [Fact]
    public async Task GetCustomersAsync_FiltersBySearchTerm()
    {
        _context.Customers.Add(new Customer { CustomerId = "BLAUS", CompanyName = "Blauer See Delikatessen" });
        _context.SaveChanges();
        var service = new CustomerService(_context);

        var result = await service.GetCustomersAsync(search: "Alfreds", TestContext.Current.CancellationToken);

        result.Should().ContainSingle();
        result[0].CustomerId.Should().Be("ALFKI");
    }

    [Fact]
    public async Task GetCustomersAsync_WithNoMatch_ReturnsEmptyList()
    {
        var service = new CustomerService(_context);

        var result = await service.GetCustomersAsync(search: "NoSuchCompany", TestContext.Current.CancellationToken);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCustomerDetailAsync_ComputesTotalValueIncludingDiscount()
    {
        var service = new CustomerService(_context);

        var result = await service.GetCustomerDetailAsync("ALFKI", TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Orders.Should().ContainSingle();

        var order = result.Orders[0];

        order.TotalValue.Should().Be(86m);
        order.ProductCount.Should().Be(2);
    }

    [Fact]
    public async Task GetCustomerDetailAsync_WithUnknownId_ReturnsNull()
    {
        var service = new CustomerService(_context);

        var result = await service.GetCustomerDetailAsync("NONE", TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCustomerDetailAsync_WithCustomerWithNoOrders_ReturnsEmptyOrderList()
    {
        _context.Customers.Add(new Customer { CustomerId = "NOORD", CompanyName = "No Orders Co" });
        _context.SaveChanges();
        var service = new CustomerService(_context);

        var result = await service.GetCustomerDetailAsync("NOORD", TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Orders.Should().BeEmpty();
    }
}
