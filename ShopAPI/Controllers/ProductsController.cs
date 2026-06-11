using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopAPI.Models;
using ShopAPI.Repositories;
using ShopAPI.Services;

namespace ShopAPI.Controllers;

/// <summary>
/// Manages product CRUD operations.
/// Supports filtering by category and full HATEOAS linking.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;
    private readonly ILinkService _links;

    /// <summary>Injects required services.</summary>
    public ProductsController(
        IProductRepository products,
        ICategoryRepository categories,
        ILinkService links)
    {
        _products = products;
        _categories = categories;
        _links = links;
    }

    /// <summary>Returns all products, optionally filtered by category.</summary>
    /// <param name="categoryId">Optional category filter.</param>
    [HttpGet]
    [ResponseCache(Duration = 60)]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? categoryId)
    {
        var items = categoryId.HasValue
            ? await _products.GetByCategoryAsync(categoryId.Value)
            : await _products.GetAllAsync();

        var response = items.Select(p => MapToResponse(p));
        return Ok(response);
    }

    /// <summary>Returns a single product by ID.</summary>
    /// <param name="id">Product ID.</param>
    [HttpGet("{id:int}")]
    [ResponseCache(Duration = 60)]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _products.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(MapToResponse(product));
    }

    /// <summary>Creates a new product.</summary>
    /// <param name="request">Product data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        if (!await _categories.ExistsAsync(request.CategoryId))
            return NotFound(new { error = $"Category {request.CategoryId} not found" });

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId
        };

        var created = await _products.CreateAsync(product);
        var full = await _products.GetByIdAsync(created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(full!));
    }

    /// <summary>Fully replaces a product.</summary>
    /// <param name="id">Product ID to update.</param>
    /// <param name="request">Replacement data.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        var existing = await _products.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (!await _categories.ExistsAsync(request.CategoryId))
            return NotFound(new { error = $"Category {request.CategoryId} not found" });

        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.Price = request.Price;
        existing.Stock = request.Stock;
        existing.CategoryId = request.CategoryId;

        var updated = await _products.UpdateAsync(existing);
        var full = await _products.GetByIdAsync(updated.Id);
        return Ok(MapToResponse(full!));
    }

    /// <summary>Deletes a product by ID.</summary>
    /// <param name="id">Product ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _products.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    // --- Private helpers ---

    private ProductResponse MapToResponse(Product p) => new(
        p.Id, p.Name, p.Description, p.Price, p.Stock,
        p.CategoryId, p.Category?.Name ?? "",
        p.CreatedAt, _links.GetProductLinks(p.Id)
    );
}
