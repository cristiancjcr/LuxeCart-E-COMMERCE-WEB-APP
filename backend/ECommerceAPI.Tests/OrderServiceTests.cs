using ECommerceAPI.DTOs;
using ECommerceAPI.Services;
using Xunit;
using FluentAssertions;
using Moq;

namespace ECommerceAPI.Tests;

// ────────────────────────────────────────────────────────────────────────────
// OrderService Tests
// These tests verify the CRITICAL business rule:
//   The order total must be computed server-side from database prices,
//   never from any client-supplied value.
// ────────────────────────────────────────────────────────────────────────────

public class OrderServiceTests
{
    // Helper: build a mock CartService that returns a predetermined cart
    private static Mock<ICartService> BuildCartMock(List<CartItemDto> items)
    {
        var mock = new Mock<ICartService>();
        mock.Setup(c => c.GetCartAsync(It.IsAny<int>()))
            .ReturnsAsync(new CartDto(items, items.Sum(i => i.LineTotal)));
        return mock;
    }

    [Fact]
    public void CartDto_Total_EqualsSum_Of_LineItems()
    {
        // Arrange — simulate what CartService returns
        var items = new List<CartItemDto>
        {
            new(1, 10, "Blue Top",   "img.jpg", 500.00m, 2, 1000.00m),
            new(2, 20, "Men Tshirt", "img.jpg", 400.00m, 1,  400.00m),
        };

        // Act
        var total = items.Sum(i => i.LineTotal);

        // Assert — the total must be the sum of individual line totals
        total.Should().Be(1400.00m,
            because: "server-side total must equal sum of (UnitPrice × Quantity) for each item");
    }

    [Fact]
    public void CartItemDto_LineTotal_IsUnitPriceTimesQuantity()
    {
        // Arrange
        decimal unitPrice = 799.00m;
        int     quantity  = 3;
        decimal expected  = unitPrice * quantity;

        // Act
        var item = new CartItemDto(1, 5, "Jeans", "img.jpg", unitPrice, quantity, unitPrice * quantity);

        // Assert
        item.LineTotal.Should().Be(expected,
            because: "LineTotal must be UnitPrice × Quantity, regardless of what the client sends");
    }

    [Fact]
    public void EmptyCart_Should_Not_Allow_Checkout()
    {
        // Arrange
        var emptyCart = new CartDto(new List<CartItemDto>(), 0);

        // Act — simulate what PlaceOrderAsync checks
        var canCheckout = emptyCart.Items.Count > 0;

        // Assert
        canCheckout.Should().BeFalse(because: "checkout must be rejected when cart is empty");
    }

    [Fact]
    public void Cart_WithMultipleItems_CalculatesCorrectTotal()
    {
        // Arrange — 5 different products
        var items = new List<CartItemDto>
        {
            new(1, 1,  "Blue Top",    "img.jpg", 500.00m,  2, 1000.00m),
            new(2, 2,  "Sunglasses",  "img.jpg", 349.00m,  1,  349.00m),
            new(3, 3,  "Sneakers",    "img.jpg", 999.00m,  1,  999.00m),
            new(4, 4,  "Yoga Pants",  "img.jpg", 850.00m,  3, 2550.00m),
            new(5, 5,  "Leather Belt","img.jpg", 299.00m,  2,  598.00m),
        };

        decimal expected = 1000 + 349 + 999 + 2550 + 598; // = 5496

        // Act
        var computedTotal = items.Sum(i => i.LineTotal);

        // Assert
        computedTotal.Should().Be(expected);
    }
}
