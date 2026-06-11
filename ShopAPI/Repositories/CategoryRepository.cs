using Microsoft.EntityFrameworkCore;
using ShopAPI.Data;
using ShopAPI.Models;

namespace ShopAPI.Repositories;

/// <summary>EF Core implementation of <see cref="ICategoryRepository"/>.</summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    /// <summary>Injects the database context.</summary>
    public CategoryRepository(AppDbContext db) => _db = db;

    /// <inheritdoc/>
    public async Task<IEnumerable<Category>> GetAllAsync() =>
        await _db.Categories.Include(c => c.Products).AsNoTracking().ToListAsync();

    /// <inheritdoc/>
    public async Task<Category?> GetByIdAsync(int id) =>
        await _db.Categories.Include(c => c.Products).AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    /// <inheritdoc/>
    public async Task<Category> CreateAsync(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return category;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category is null) return false;
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(int id) =>
        await _db.Categories.AnyAsync(c => c.Id == id);
}
