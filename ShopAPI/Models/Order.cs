namespace ShopAPI.Models;

/// <summary>Order status values.</summary>
public enum OrderStatus { Pending, Confirmed, Shipped, Delivered, Cancelled }

/// <summary>Customer order entity.</summary>
public class Order
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }
    /// <summary>Customer full name.</summary>
    public string CustomerName { get; set; } = string.Empty;
    /// <summary>Customer email address.</summary>
    public string CustomerEmail { get; set; } = string.Empty;
    /// <summary>Current order status.</summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    /// <summary>When the order was placed (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>Line items belonging to this order.</summary>
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    /// <summary>Computed total from all items.</summary>
    public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
}
