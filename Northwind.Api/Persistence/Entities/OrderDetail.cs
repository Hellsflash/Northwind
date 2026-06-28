using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Api.Persistence.Entities;

[Table("Order Details")]
public class OrderDetail
{
    [Column("OrderID")]
    public int OrderId { get; set; }

    [Column("ProductID")]
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public float Discount { get; set; }
    public Order? Order { get; set; }
}
