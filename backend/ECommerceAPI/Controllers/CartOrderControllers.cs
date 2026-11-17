using System.Security.Claims;
using ECommerceAPI.DTOs;
using ECommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

// ── CartController ────────────────────────────────────────────────────────────
[ApiController]
[Route("api/cart")]
[Authorize]   // all cart endpoints require a valid JWT
public class CartController : ControllerBase
{
    private readonly ICartService _cart;

    public CartController(ICartService cart) => _cart = cart;

    // Helper: extract userId from the JWT "sub" claim
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET api/cart
    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(await _cart.GetCartAsync(UserId));

    // POST api/cart
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddToCartRequest req)
    {
        if (req.Quantity <= 0)
            return BadRequest(new { message = "Quantity must be at least 1." });

        await _cart.AddOrUpdateAsync(UserId, req.ProductId, req.Quantity);
        return Ok(await _cart.GetCartAsync(UserId));
    }

    // PUT api/cart/{productId}
    [HttpPut("{productId:int}")]
    public async Task<IActionResult> Update(int productId, [FromBody] UpdateCartRequest req)
    {
        await _cart.UpdateQuantityAsync(UserId, productId, req.Quantity);
        return Ok(await _cart.GetCartAsync(UserId));
    }

    // DELETE api/cart/{productId}
    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> Remove(int productId)
    {
        await _cart.RemoveItemAsync(UserId, productId);
        return Ok(await _cart.GetCartAsync(UserId));
    }
}
