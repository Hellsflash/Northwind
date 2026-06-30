using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Api.Persistence;

namespace Northwind.Tests;

public class NorthwindApiFactory : WebApplicationFactory<Program>
{
    // Set environment variable before the factory builds
    public NorthwindApiFactory()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // Remove the DbContextOptions registration
            var descriptor = services.FirstOrDefault(s => 
                s.ServiceType == typeof(DbContextOptions<NorthwindContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add InMemory context for testing
            services.AddDbContext<NorthwindContext>(options =>
                options.UseInMemoryDatabase("IntegrationTestDb"));
        });
    }
}

