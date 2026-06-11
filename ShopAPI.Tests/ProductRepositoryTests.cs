using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ShopAPI.Data;
using ShopAPI.Models;
using ShopAPI.Repositories;
using Xunit;

namespace ShopAPI.Tests;

/// <summary>
/// Unit tests for ProductRepository using an in-memory SQLite database.
/// Each test gets a fresh database — tests are isolated and repeatable.
/// </summary>
public class ProductRepositoryTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly ProductRepository _repo;

    /// <summary>Sets up a fresh in-memory database before each test.</summary>
    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique per test
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();

        // Seed test data
        _db.Categories.Add(new Category { Id = 1, Name = "Test Category" });
        _db.SaveChanges();

        _repo = new ProductRepository(_db);
    }

    /// <summary>GetAllAsync should return all seeded products.</summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        _db.Products.AddRange(
            new Product { Name = "A", Price = 10, Stock = 5, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Name = "B", Price = 20, Stock = 3, CategoryId = 1, CreatedAt = DateTime.UtcNow }
        );
        await _db.SaveChangesAsync();

        var result = await _repo.GetAllAsync();

        result.Should().HaveCount(2);
    }

    /// <summary>GetByIdAsync should return the correct product.</summary>
    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsProduct()
    {
        _db.Products.Add(new Product { Id = 99, Name = "Found", Price = 50, Stock = 1, CategoryId = 1, CreatedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();

        var result = await _repo.GetByIdAsync(99);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Found");
    }

    /// <summary>GetByIdAsync should return null for a missing product.</summary>
    [Fact]
    public async Task GetByIdAsync_MissingId_ReturnsNull()
    {
        var result = await _repo.GetByIdAsync(9999);
        result.Should().BeNull();
    }

    /// <summary>CreateAsync should persist a product and assign an ID.</summary>
    [Fact]
    public async Task CreateAsync_ValidProduct_PersistsAndReturnsWithId()
    {
        var product = new Product { Name = "New", Price = 15, Stock = 10, CategoryId = 1, CreatedAt = DateTime.UtcNow };

        var created = await _repo.CreateAsync(product);

        created.Id.Should().BeGreaterThan(0);
        _db.Products.Should().ContainSingle(p => p.Name == "New");
    }

    /// <summary>DeleteAsync should remove an existing product and return true.</summary>
    [Fact]
    public async Task DeleteAsync_ExistingProduct_ReturnsTrueAndRemoves()
    {
        _db.Products.Add(new Product { Id = 55, Name = "ToDelete", Price = 5, Stock = 1, CategoryId = 1, CreatedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();

        var result = await _repo.DeleteAsync(55);

        result.Should().BeTrue();
        _db.Products.Should().NotContain(p => p.Id == 55);
    }

    /// <summary>DeleteAsync should return false for a non-existent product.</summary>
    [Fact]
    public async Task DeleteAsync_MissingProduct_ReturnsFalse()
    {
        var result = await _repo.DeleteAsync(9999);
        result.Should().BeFalse();
    }

    /// <summary>ExistsAsync should return true when the product exists.</summary>
    [Fact]
    public async Task ExistsAsync_ExistingProduct_ReturnsTrue()
    {
        _db.Products.Add(new Product { Id = 77, Name = "Exists", Price = 1, Stock = 1, CategoryId = 1, CreatedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();

        var result = await _repo.ExistsAsync(77);
        result.Should().BeTrue();
    }

    /// <summary>Cleans up the in-memory database after each test.</summary>
    public void Dispose() => _db.Dispose();
}
