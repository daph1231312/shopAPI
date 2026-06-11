namespace ShopAPI.Models;

/// <summary>Product entity — core item sold in the shop.</summary>
public class Product
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }
    /// <summary>Product name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Detailed description.</summary>
    public string? Description { get; set; }
    /// <summary>Price in USD.</summary>
    public decimal Price { get; set; }
    /// <summary>Available stock quantity.</summary>
    public int Stock { get; set; }
    /// <summary>Foreign key to Category.</summary>
    public int CategoryId { get; set; }
    /// <summary>Navigation property.</summary>
    public Category Category { get; set; } = null!;
    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
