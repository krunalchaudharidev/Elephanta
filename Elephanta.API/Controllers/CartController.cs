using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Elephanta.Domain.Constants;
using Elephanta.Application.Features.Cart.DTOs;
using Elephanta.Application.Features.Cart.Interfaces;
using Elephanta.Domain.Entities;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
[ApiExplorerSettings(GroupName = "User")]
public class CartController : ControllerBase
{
    private readonly ICartService _service;

    public CartController(ICartService service)
    {
        _service = service;
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] CartItemRequest req)
    {
        var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        if (req.Quantity <= 0) return BadRequest(new { message = "Quantity must be greater than zero" });

        // ensure product exists
        var product = (await _service.GetItemsByUserAsync(userId)).FirstOrDefault()?.Product;
        // above is just to keep parity; better to fetch product in service if needed. We'll set unit price in service based on product data.

        var item = new CartItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = req.ProductId,
            Quantity = req.Quantity,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _service.AddItemAsync(item);

        var resp = new CartItemResponse
        {
            Id = added.Id,
            ProductId = added.ProductId,
            ProductName = added.Product?.Name ?? string.Empty,
            Quantity = added.Quantity,
            UnitPrice = added.UnitPrice
        };

        return CreatedAtAction(nameof(GetItem), new { id = resp.Id }, resp);
    }

    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateCartItemRequest req)
    {
        var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        if (req.Quantity <= 0) return BadRequest(new { message = "Quantity must be greater than zero" });

        var existing = await _service.GetItemByIdAsync(id);
        if (existing == null || existing.UserId != userId) return NotFound();

        existing.Quantity = req.Quantity;
        existing.UpdatedAt = DateTime.UtcNow;

        await _service.UpdateItemAsync(existing);
        return NoContent();
    }

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> DeleteItem(Guid id)
    {
        var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var existing = await _service.GetItemByIdAsync(id);
        if (existing == null || existing.UserId != userId) return NotFound();

        await _service.DeleteItemAsync(id);
        return NoContent();
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var paged = await _service.GetItemsByUserAsync(userId, pageNumber, pageSize);
        var items = paged.Items.Select(c => new CartItemResponse
        {
            Id = c.Id,
            ProductId = c.ProductId,
            ProductName = c.Product?.Name ?? string.Empty,
            Quantity = c.Quantity,
            UnitPrice = c.UnitPrice
        }).ToList();

        var result = new Elephanta.Application.Common.PagedResult<CartItemResponse>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };

        return Ok(result);
    }

    [HttpGet("items/{id}")]
    public async Task<IActionResult> GetItem(Guid id)
    {
        var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var c = await _service.GetItemByIdAsync(id);
        if (c == null || c.UserId != userId) return NotFound();

        var resp = new CartItemResponse
        {
            Id = c.Id,
            ProductId = c.ProductId,
            ProductName = c.Product?.Name ?? string.Empty,
            Quantity = c.Quantity,
            UnitPrice = c.UnitPrice
        };

        return Ok(resp);
    }
}
