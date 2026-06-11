using Microsoft.EntityFrameworkCore;
using ShopAPI.Data;
using ShopAPI.Models;

namespace ShopAPI.Repositories;

/// <summary>EF Core implementation of <see cref="IOrderRepository"/>.</summary>
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    /// <summary>Injects the database context.</summary>
    public OrderRepository(AppDbContext db) => _db = db;

    /// <inheritdoc/>
    public async Task<IEnumerable<Order>> GetAllAsync() =>
        await _db.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .AsNoTracking().ToListAsync();

    /// <inheritdoc/>
    public async Task<Order?> GetByIdAsync(int id) =>
        await _db.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);

    /// <inheritdoc/>
    public async Task<Order> CreateAsync(Order order)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    /// <inheritdoc/>
    public async Task<Order> UpdateAsync(Order order)
    {
        _db.Orders.Update(order);
        await _db.SaveChangesAsync();
        return order;
    }
}
