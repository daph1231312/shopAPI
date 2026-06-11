using Microsoft.EntityFrameworkCore;
using ShopAPI.Models;

namespace ShopAPI.Data;

/// <summary>
/// Entity Framework database context for ShopAPI.
/// Represents a session with the SQLite database.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>Initializes the context with injected options.</summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>Products table.</summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>Categories table.</summary>
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>Orders table.</summary>
    public DbSet<Order> Orders => Set<Order>();

    /// <summary>Order line items table.</summary>
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    /// <summary>Configures entity relationships and seed data.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Price precision
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(18,2)");

        // Seed categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Gadgets and devices" },
            new Category { Id = 2, Name = "Clothing", Description = "Apparel and accessories" },
            new Category { Id = 3, Name = "Books", Description = "Physical and digital books" }
        );

        // Seed products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Wireless Headphones", Description = "Noise-cancelling BT headphones", Price = 79.99m, Stock = 50, CategoryId = 1, CreatedAt = new DateTime(2024, 1, 1) },
            new Product { Id = 2, Name = "Mechanical Keyboard", Description = "TKL, tactile switches", Price = 120.00m, Stock = 30, CategoryId = 1, CreatedAt = new DateTime(2024, 1, 1) },
            new Product { Id = 3, Name = "Cotton T-Shirt", Description = "Unisex, 100% organic cotton", Price = 19.99m, Stock = 200, CategoryId = 2, CreatedAt = new DateTime(2024, 1, 1) }
        );
    }
}
