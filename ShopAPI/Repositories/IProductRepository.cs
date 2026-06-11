using ShopAPI.Models;

namespace ShopAPI.Repositories;

/// <summary>
/// Defines data access operations for products.
/// Depending on this interface (not the concrete class) keeps controllers testable.
/// </summary>
public interface IProductRepository
{
    /// <summary>Returns all products including their category.</summary>
    Task<IEnumerable<Product>> GetAllAsync();

    /// <summary>Returns a single product by ID, or null if not found.</summary>
    Task<Product?> GetByIdAsync(int id);

    /// <summary>Returns all products in a given category.</summary>
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);

    /// <summary>Persists a new product.</summary>
    Task<Product> CreateAsync(Product product);

    /// <summary>Updates an existing product.</summary>
    Task<Product> UpdateAsync(Product product);

    /// <summary>Deletes a product by ID. Returns false if not found.</summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>Checks whether a product with the given ID exists.</summary>
    Task<bool> ExistsAsync(int id);
}
