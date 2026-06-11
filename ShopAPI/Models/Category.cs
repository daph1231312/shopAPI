namespace ShopAPI.Models;

/// <summary>Product category entity.</summary>
public class Category
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }
    /// <summary>Category name e.g. Electronics, Clothing.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Optional description.</summary>
    public string? Description { get; set; }
    /// <summary>Products in this category.</summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
