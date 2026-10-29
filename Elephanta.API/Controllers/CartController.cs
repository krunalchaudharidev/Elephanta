using Microsoft.AspNetCore.Mvc;
using Elephanta.API.Helpers;
using Elephanta.Application.Features.Cart.DTOs;
using Elephanta.Application.Features.Cart.Interfaces;
using Elephanta.Application.Features.Catalog.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Application.Common;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "User")]
public class CartController : ControllerBase
{
    private readonly ICartService _service;
    private readonly IProductService _productService;

    public CartController(ICartService service, IProductService productService)
    {
        _service = service;
        _productService = productService;
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] CartItemRequest req)
    {
        if (!TryGetCartOwner(req.SessionId, out var userId, out var sessionId))
            return BadRequest(new { message = "SessionId is required for guest carts" });

        if (req.Quantity <= 0) return BadRequest(new { message = "Quantity must be greater than zero" });

        var product = await _productService.GetProductByIdAsync(req.ProductId);
        if (product == null) return NotFound(new { message = "Product not found" });

        var item = new CartItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SessionId = sessionId,
            ProductId = req.ProductId,
            Quantity = req.Quantity,
            UnitPrice = product.Price,
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
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateCartItemRequest req, [FromQuery] string? sessionId)
    {
        if (!TryGetCartOwner(sessionId, out var userId, out sessionId))
            return BadRequest(new { message = "SessionId is required for guest carts" });

        if (req.Quantity <= 0) return BadRequest(new { message = "Quantity must be greater than zero" });

        var existing = await _service.GetItemByIdAsync(id);
        if (existing == null || !IsOwnedBy(existing, userId, sessionId)) return NotFound();

        var product = await _productService.GetProductByIdAsync(existing.ProductId);
        if (product == null) return NotFound(new { message = "Product not found" });

        existing.Quantity = req.Quantity;
        existing.UnitPrice = product.Price;
        existing.UpdatedAt = DateTime.UtcNow;

        await _service.UpdateItemAsync(existing);
        return Ok(new ApiResponse(true, "Cart item updated successfully", existing.Id));
    }

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> DeleteItem(Guid id, [FromQuery] string? sessionId)
    {
        if (!TryGetCartOwner(sessionId, out var userId, out sessionId))
            return BadRequest(new { message = "SessionId is required for guest carts" });

        var existing = await _service.GetItemByIdAsync(id);
        if (existing == null || !IsOwnedBy(existing, userId, sessionId)) return NotFound();

        await _service.DeleteItemAsync(id);
        return Ok(new ApiResponse(true, "Cart item deleted successfully", id));
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sessionId = null)
    {
        if (!TryGetCartOwner(sessionId, out var userId, out sessionId))
            return BadRequest(new { message = "SessionId is required for guest carts" });

        var paged = await _service.GetItemsAsync(userId, sessionId, pageNumber, pageSize);
        var items = paged.Items.Select(c => new CartItemResponse
        {
            Id = c.Id,
            ProductId = c.ProductId,
            ProductName = c.Product?.Name ?? string.Empty,
            PrimaryImageId = c.Product?.Images?.FirstOrDefault(image => image.IsPrimary && image.MediaId.HasValue)?.MediaId,
            Quantity = c.Quantity,
            UnitPrice = c.Product?.Price ?? c.UnitPrice
        }).ToList();

        var result = new PagedResult<CartItemResponse>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };

        return Ok(result);
    }

    [HttpGet("items/{id}")]
    public async Task<IActionResult> GetItem(Guid id, [FromQuery] string? sessionId)
    {
        if (!TryGetCartOwner(sessionId, out var userId, out sessionId))
            return BadRequest(new { message = "SessionId is required for guest carts" });

        var c = await _service.GetItemByIdAsync(id);
        if (c == null || !IsOwnedBy(c, userId, sessionId)) return NotFound();

        var resp = new CartItemResponse
        {
            Id = c.Id,
            ProductId = c.ProductId,
            ProductName = c.Product?.Name ?? string.Empty,
            PrimaryImageId = c.Product?.Images?.FirstOrDefault(image => image.IsPrimary && image.MediaId.HasValue)?.MediaId,
            Quantity = c.Quantity,
            UnitPrice = c.Product?.Price ?? c.UnitPrice,
        };

        return Ok(resp);
    }

    private bool TryGetCartOwner(string? requestedSessionId, out Guid? userId, out string? sessionId)
    {
        userId = ClaimsHelper.GetUserIdFromClaims(User);
        sessionId = userId.HasValue ? null : requestedSessionId;
        return userId.HasValue || !string.IsNullOrWhiteSpace(sessionId);
    }

    private static bool IsOwnedBy(CartItem item, Guid? userId, string? sessionId)
    {
        return userId.HasValue
            ? item.UserId == userId && item.SessionId == null
            : item.UserId == null && item.SessionId == sessionId;
    }
}
