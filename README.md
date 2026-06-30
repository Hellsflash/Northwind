# Northwind Customer Lookup Tool

A small internal tool for Northwind Traders staff to look up customers and review
their order history, built as an ASP.NET Core Minimal API backend with a Blazor
Web App front end.

## Solution structure

```
Northwind.sln
├── Northwind.Api/        ASP.NET Core Minimal API (the core deliverable)
├── Northwind.Web/        Blazor Web App, server-rendered 
└── Northwind.Tests/      Unit tests (service layer) + integration tests (endpoints)
```

## Running it

**Prerequisites:** .NET 10 SDK, SQL Server LocalDB, the Northwind sample database
restored into a local instance named `Northwind` (the standard
[`instnwnd.sql`](https://github.com/microsoft/sql-server-samples/tree/master/samples/databases/northwind-pubs)
script)

1. Update the connection string in `Northwind.Api/appsettings.json` if your LocalDB
   instance name or database name differs from the default.
2. Run the API: `dotnet run --project Northwind.Api` (defaults to `https://localhost:7050`)
3. Run the Blazor app: `dotnet run --project Northwind.Web` (defaults to `https://localhost:7100`)
4. Browse to the Blazor app's URL, or hit the API directly via its Swagger UI
   (`/swagger`, in Development)

   If your ports differ from the defaults above, update `ApiBaseUrl` in
`Northwind.Web/appsettings.json` and `BlazorAppUrl` in `Northwind.Api/appsettings.json`
to match. The api project has a corse policy only acceptiong calls form `BlazorAppUrl`

# Scope:

1. **Customer Overview** — list of customers with name + order count, searchable/filterable by name.
2. **Customer Detail** — a customer's details + order history, where each order shows total value and number of products.

Both are implemented as Minimal API endpoints:

- `GET /api/customers?search={term}` → list of `{ CustomerId, CompanyName, OrderCount }`
- `GET /api/customers/{id}` → customer detail + `Orders: [{ OrderId, OrderDate, TotalValue, ProductCount }]`

## Decisions and assumptions worth calling out

- **"Customer name" for search/filter** I assumed that customer name is `CompanyName`, not `ContactName`
  Northwind is a B2B context (companies ordering from a supplier), so company name felt like the more natural match for "customer name" in a staff lookup tool

- **"Total value of the order"** is computed discount-inclusive:
  `SUM(UnitPrice * Quantity * (1 - Discount))` across all line items. This reflects actual realized revenue rather than list-price revenue

- **"Number of products"** My assumption on number of products was that the count of *distinct* products on the order,
  not the total unit quantity across line items (e.g. an order of 5 units of oneproduct and 3 of another counts as 2 products, not 8 units)

- **No pagination** on the customer list or order history. Northwind tops out at 91 customers and ~30 orders per customer, so this isn't a real-world concern at this data volume.
  Flagging it because it's the kind of thing that would need addressing before this scaled to a larger customer base — `Skip`/`Take` on the customer list would be the natural extension point.

- **No authentication/authorization.** The brief describes an internal staff tool without specifying access control requirements, so this is intentionally out of scope rather than an oversight.
  In a real deployment I'd put this behind the organization's existing internal auth and would add role-based restriction if customer/order data turns out to have reporting sensitivity.

- **No repository layer over EF Core.** I skipped the repository pattern over EF Core — `DbSet<T>` already acts as a repository/unit-of-work, so wrapping it again adds indirection without benefit.
  Business logic instead lives in `CustomerService`, behind an `ICustomerService` interface, which is the abstraction that actually matters.

- **The Blazor app duplicates the API's DTOs as its own records** rather than referencing a shared contracts project. For two endpoints, a shared assembly is more ceremony than the duplication it avoids.
  If the API grows, extracting `Northwind.Contracts` would be the natural next step.


  ## What I'd do next with more time

- Pagination on the customer list for scale.
- Sorting options on order history (by date, by value).
- A shared contracts project if the API surface grows beyond these two endpoints.
- Testcontainers-based integration tests against a real SQL Server instance.
- Structured logging and basic request/response logging middleware for
  observability in a real deployment.
