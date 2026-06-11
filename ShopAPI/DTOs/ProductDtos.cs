using System.ComponentModel.DataAnnotations;

namespace ShopAPI.DTOs;

/// <summary>Read-only product response with HATEOAS links.</summary>
public record ProductResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    int CategoryId,
    string CategoryName,
    DateTime CreatedAt,
    Dictionary<string, string> Links
);

/// <summary>Payload for creating a new product.</summary>
public record CreateProductRequest(
    [Required, MinLength(2), MaxLength(200)] string Name,
    string? Description,
    [Range(0.01, 99999.99)] decimal Price,
    [Range(0, 100000)] int Stock,
    [Range(1, int.MaxValue)] int CategoryId
);

/// <summary>Payload for fully updating a product.</summary>
public record UpdateProductRequest(
    [Required, MinLength(2), MaxLength(200)] string Name,
    string? Description,
    [Range(0.01, 99999.99)] decimal Price,
    [Range(0, 100000)] int Stock,
    [Range(1, int.MaxValue)] int CategoryId
);
