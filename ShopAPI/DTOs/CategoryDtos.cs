using System.ComponentModel.DataAnnotations;

namespace ShopAPI.DTOs;

/// <summary>Category response with HATEOAS links.</summary>
public record CategoryResponse(
    int Id,
    string Name,
    string? Description,
    int ProductCount,
    Dictionary<string, string> Links
);

/// <summary>Payload for creating a category.</summary>
public record CreateCategoryRequest(
    [Required, MinLength(2), MaxLength(100)] string Name,
    string? Description
);
