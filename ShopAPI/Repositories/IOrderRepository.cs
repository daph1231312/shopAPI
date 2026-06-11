using ShopAPI.Models;

namespace ShopAPI.Repositories;

/// <summary>Data access operations for orders.</summary>
public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
}
