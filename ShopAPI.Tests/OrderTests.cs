using FluentAssertions;
using ShopAPI.Models;
using Xunit;

namespace ShopAPI.Tests;

/// <summary>Unit tests for Order model logic.</summary>
public class OrderTests
{
    /// <summary>Total should be the sum of (unitPrice * quantity) for all items.</summary>
    [Fact]
    public void Total_WithMultipleItems_ReturnsCorrectSum()
    {
        var order = new Order
        {
            CustomerName = "Alice",
            CustomerEmail = "alice@test.com",
            Items = new List<OrderItem>
            {
                new() { ProductId = 1, Quantity = 2, UnitPrice = 10.00m },
                new() { ProductId = 2, Quantity = 1, UnitPrice = 25.50m }
            }
        };

        order.Total.Should().Be(45.50m);
    }

    /// <summary>Total should be zero for an empty order.</summary>
    [Fact]
    public void Total_NoItems_ReturnsZero()
    {
        var order = new Order { CustomerName = "Bob", CustomerEmail = "bob@test.com" };
        order.Total.Should().Be(0m);
    }

    /// <summary>Default status should be Pending.</summary>
    [Fact]
    public void NewOrder_DefaultStatus_IsPending()
    {
        var order = new Order();
        order.Status.Should().Be(OrderStatus.Pending);
    }
}
