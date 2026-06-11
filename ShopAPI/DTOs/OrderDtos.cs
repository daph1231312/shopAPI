using System.ComponentModel.DataAnnotations;
using ShopAPI.Models;

namespace ShopAPI.DTOs;

/// <summary>Order item inside a request.</summary>
public record OrderItemRequest(
    [Range(1, int.MaxValue)] int ProductId,
    [Range(1, 1000)] int Quantity
);

/// <summary>Order item in a response.</summary>
public record OrderItemResponse(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal
);

/// <summary>Full order response with HATEOAS links.</summary>
public record OrderResponse(
    int Id,
    string CustomerName,
    string CustomerEmail,
    string Status,
    decimal Total,
    DateTime CreatedAt,
    IEnumerable<OrderItemResponse> Items,
    Dictionary<string, string> Links
);

/// <summary>Payload for placing a new order.</summary>
public record CreateOrderRequest(
    [Required, MinLength(2), MaxLength(200)] string CustomerName,
    [Required, EmailAddress] string CustomerEmail,
    [Required, MinLength(1)] IEnumerable<OrderItemRequest> Items
);

/// <summary>Payload for updating order status only.</summary>
public record UpdateOrderStatusRequest(OrderStatus Status);
