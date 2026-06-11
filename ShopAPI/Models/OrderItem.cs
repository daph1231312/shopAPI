namespace ShopAPI.Models;

/// <summary>A single line item within an order.</summary>
public class OrderItem
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }
    /// <summary>Foreign key to Order.</summary>
    public int OrderId { get; set; }
    /// <summary>Navigation property.</summary>
    public Order Order { get; set; } = null!;
    /// <summary>Foreign key to Product.</summary>
    public int ProductId { get; set; }
    /// <summary>Navigation property.</summary>
    public Product Product { get; set; } = null!;
    /// <summary>Quantity ordered.</summary>
    public int Quantity { get; set; }
    /// <summary>Price snapshot at purchase time.</summary>
    public decimal UnitPrice { get; set; }
}
