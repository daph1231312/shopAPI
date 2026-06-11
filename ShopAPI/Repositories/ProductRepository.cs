using Microsoft.EntityFrameworkCore;
using ShopAPI.Data;
using ShopAPI.Models;

namespace ShopAPI.Repositories;

/// <summary>
/// Concrete EF Core implementation of <see cref="IProductRepository"/>.
/// All database queries live here — controllers never touch DbContext directly.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    /// <summary>Constructor — AppDbContext injected by DI container.</summary>
    public ProductRepository(AppDbContext db) => _db = db;

    /// <inheritdoc/>
    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _db.Products.Include(p => p.Category).AsNoTracking().ToListAsync();

    /// <inheritdoc/>
    public async Task<Product?> GetByIdAsync(int id) =>
        await _db.Products.Include(p => p.Category).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    /// <inheritdoc/>
    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId) =>
        await _db.Products.Include(p => p.Category).Where(p => p.CategoryId == categoryId).AsNoTracking().ToListAsync();

    /// <inheritdoc/>
    public async Task<Product> CreateAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    /// <inheritdoc/>
    public async Task<Product> UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
        return product;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return false;
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(int id) =>
        await _db.Products.AnyAsync(p => p.Id == id);
}
