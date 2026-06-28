using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Api.Persistence.Entities;

[Table("Customers")]
public class Customer
{
    [Key]
    [Column("CustomerID")]
    [MaxLength(5)]
    public required string CustomerId { get; set; }
    public required string CompanyName { get; set; }
    public string? ContactName { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public ICollection<Order> Orders { get; set; } = [];
}
