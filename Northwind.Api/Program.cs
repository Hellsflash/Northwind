using Microsoft.EntityFrameworkCore;
using Northwind.Api.Endpoints;
using Northwind.Api.Interfaces;
using Northwind.Api.Persistence;
using Northwind.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Skip SqlServer registration in test environments - the test factory will register InMemory
if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddDbContext<NorthwindContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Northwind")));
}

builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {

        policy.WithOrigins(builder.Configuration["BlazorAppUrl"] ?? "https://localhost:7100")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 7050;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorApp");

app.MapCustomerEndPoints();

app.Run();
