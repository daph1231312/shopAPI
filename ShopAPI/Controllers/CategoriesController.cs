using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopAPI.Models;
using ShopAPI.Repositories;
using ShopAPI.Services;

namespace ShopAPI.Controllers;

/// <summary>Manages product categories.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categories;
    private readonly ILinkService _links;

    /// <summary>Injects repository and link service.</summary>
    public CategoriesController(ICategoryRepository categories, ILinkService links)
    {
        _categories = categories;
        _links = links;
    }

    /// <summary>Returns all categories with product counts.</summary>
    [HttpGet]
    [ResponseCache(Duration = 120)]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var items = await _categories.GetAllAsync();
        return Ok(items.Select(MapToResponse));
    }

    /// <summary>Returns a single category by ID.</summary>
    /// <param name="id">Category ID.</param>
    [HttpGet("{id:int}")]
    [ResponseCache(Duration = 120)]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categories.GetByIdAsync(id);
        return category is null ? NotFound() : Ok(MapToResponse(category));
    }

    /// <summary>Creates a new category.</summary>
    /// <param name="request">Category data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var category = new Category { Name = request.Name, Description = request.Description };
        var created = await _categories.CreateAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
    }

    /// <summary>Deletes a category. Fails if it has associated products.</summary>
    /// <param name="id">Category ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categories.GetByIdAsync(id);
        if (category is null) return NotFound();
        if (category.Products.Any())
            return Conflict(new { error = "Cannot delete a category that has products." });

        await _categories.DeleteAsync(id);
        return NoContent();
    }

    private CategoryResponse MapToResponse(Category c) => new(
        c.Id, c.Name, c.Description, c.Products.Count, _links.GetCategoryLinks(c.Id)
    );
}
