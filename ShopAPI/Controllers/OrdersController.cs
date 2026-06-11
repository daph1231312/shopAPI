using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopAPI.Models;
using ShopAPI.Repositories;
using ShopAPI.Services;

namespace ShopAPI.Controllers;

/// <summary>Handles order placement and status management.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orders;
    private readonly IProductRepository _products;
    private readonly ILinkService _links;

    /// <summary>Injects required repositories and link service.</summary>
    public OrdersController(IOrderRepository orders, IProductRepository products, ILinkService links)
    {
        _orders = orders;
        _products = products;
        _links = links;
    }

    /// <summary>Returns all orders.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var items = await _orders.GetAllAsync();
        return Ok(items.Select(MapToResponse));
    }

    /// <summary>Returns a single order by ID.</summary>
    /// <param name="id">Order ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        return order is null ? NotFound() : Ok(MapToResponse(order));
    }

    /// <summary>Places a new order. Validates stock and snapshots prices.</summary>
    /// <param name="request">Order details including customer info and items.</param>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var order = new Order
        {
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail
        };

        foreach (var item in request.Items)
        {
            var product = await _products.GetByIdAsync(item.ProductId);
            if (product is null)
                return BadRequest(new { error = $"Product {item.ProductId} not found" });
            if (product.Stock < item.Quantity)
                return BadRequest(new { error = $"Insufficient stock for '{product.Name}'" });

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price   // price snapshot
            });

            product.Stock -= item.Quantity;
            await _products.UpdateAsync(product);
        }

        var created = await _orders.CreateAsync(order);
        var full = await _orders.GetByIdAsync(created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(full!));
    }

    /// <summary>Updates only the status of an order (e.g. Pending → Confirmed).</summary>
    /// <param name="id">Order ID.</param>
    /// <param name="request">New status value.</param>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order is null) return NotFound();

        order.Status = request.Status;
        await _orders.UpdateAsync(order);
        var full = await _orders.GetByIdAsync(id);
        return Ok(MapToResponse(full!));
    }

    private OrderResponse MapToResponse(Order o) => new(
        o.Id,
        o.CustomerName,
        o.CustomerEmail,
        o.Status.ToString(),
        o.Total,
        o.CreatedAt,
        o.Items.Select(i => new OrderItemResponse(
            i.ProductId,
            i.Product?.Name ?? "",
            i.Quantity,
            i.UnitPrice,
            i.Quantity * i.UnitPrice
        )),
        _links.GetOrderLinks(o.Id)
    );
}
