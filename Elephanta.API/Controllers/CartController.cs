using Elephanta.API.Helpers;
using Elephanta.Application.Common;
using Elephanta.Application.Features.Cart.DTOs;
using Elephanta.Application.Features.Cart.Interfaces;
using Elephanta.Application.Features.Catalog.Interfaces;
using Elephanta.Application.Features.Offers.Interfaces;
using Elephanta.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using CartEntity = Elephanta.Domain.Entities.Cart;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "User")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IProductService _productService;
    private readonly IOfferService _offerService;

    public CartController(ICartService cartService, IProductService productService, IOfferService offerService)
    {
        _cartService = cartService;
        _productService = productService;
        _offerService = offerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart([FromQuery] string? sessionId)
    {
        if (!TryGetCartOwner(sessionId, out var userId, out var resolvedSessionId))
            return BadRequest(new ApiResponse(false, "SessionId is required for guest carts"));

        var cart = await _cartService.GetOrCreateCartAsync(userId, resolvedSessionId);
        return Ok(ToCartResponse(cart));
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] CartItemRequest req)
    {
        if (!TryGetCartOwner(req.SessionId, out var userId, out var sessionId))
            return BadRequest(new ApiResponse(false, "SessionId is required for guest carts"));

        if (req.Quantity <= 0)
            return BadRequest(new ApiResponse(false, "Quantity must be greater than zero"));

        var product = await _productService.GetProductByIdAsync(req.ProductId);
        if (product == null)
            return NotFound(new ApiResponse(false, "Product not found"));

        var cart = await _cartService.GetOrCreateCartAsync(userId, sessionId);
        var existing = await _cartService.GetItemByProductIdAsync(req.ProductId, cart.Id);
        var resultingQuantity = (existing?.Quantity ?? 0) + req.Quantity;
        if (resultingQuantity > product.StockQuantity)
            return BadRequest(new ApiResponse(false, $"Insufficient stock. Available quantity: {product.StockQuantity}"));

        CartItem added;
        var created = existing == null;
        if (existing != null)
        {
            existing.Quantity = resultingQuantity;
            existing.UnitPrice = product.Price;
            existing.UpdatedAt = DateTime.UtcNow;
            await _cartService.UpdateItemAsync(existing);
            added = existing;
        }
        else
        {
            added = await _cartService.AddItemToCartAsync(new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = req.ProductId,
                Quantity = req.Quantity,
                UnitPrice = product.Price,
                CreatedAt = DateTime.UtcNow
            });
        }

        var saved = await _cartService.GetItemByIdAsync(added.Id, cart.Id) ?? added;
        var response = ToItemResponse(saved);
        return created
            ? CreatedAtAction(nameof(GetItem), new { id = response.Id, sessionId }, response)
            : Ok(response);
    }

    [HttpPut("items/{id:guid}")]
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateCartItemRequest req, [FromQuery] string? sessionId)
    {
        if (!TryGetCartOwner(sessionId, out var userId, out var resolvedSessionId))
            return BadRequest(new { message = "SessionId is required for guest carts" });

        if (req.Quantity <= 0)
            return BadRequest(new ApiResponse(false, "Quantity must be greater than zero"));

        var cart = await _cartService.GetCartAsync(userId, resolvedSessionId);
        if (cart == null) return NotFound(new ApiResponse(false, "Cart not found"));

        var item = await _cartService.GetItemByIdAsync(id, cart.Id);
        if (item == null) return NotFound(new ApiResponse(false, "Cart item not found"));

        var product = await _productService.GetProductByIdAsync(item.ProductId);
        if (product == null)
            return NotFound(new ApiResponse(false, "Product not found"));
        if (req.Quantity > product.StockQuantity)
            return BadRequest(new ApiResponse(false, $"Insufficient stock. Available quantity: {product.StockQuantity}"));

        item.Quantity = req.Quantity;
        item.UnitPrice = product.Price;
        item.UpdatedAt = DateTime.UtcNow;
        await _cartService.UpdateItemAsync(item);

        var updated = await _cartService.GetItemByIdAsync(id, cart.Id) ?? item;
        return Ok(ToItemResponse(updated));
    }

    [HttpDelete("items/{id:guid}")]
    public async Task<IActionResult> DeleteItem(Guid id, [FromQuery] string? sessionId)
    {
        if (!TryGetCartOwner(sessionId, out var userId, out var resolvedSessionId))
            return BadRequest(new ApiResponse(false, "SessionId is required for guest carts"));

        var cart = await _cartService.GetCartAsync(userId, resolvedSessionId);
        if (cart == null) return NotFound(new ApiResponse(false, "Cart not found"));

        var item = await _cartService.GetItemByIdAsync(id, cart.Id);
        if (item == null) return NotFound(new ApiResponse(false, "Cart item not found"));

        await _cartService.DeleteItemAsync(id, cart.Id);
        return Ok(new ApiResponse(true, "Cart item deleted successfully", id));
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sessionId = null)
    {
        if (!TryGetCartOwner(sessionId, out var userId, out var resolvedSessionId))
            return BadRequest(new ApiResponse(false, "SessionId is required for guest carts"));
        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest(new ApiResponse(false, "Page number and page size must be greater than zero"));

        var cart = await _cartService.GetOrCreateCartAsync(userId, resolvedSessionId);
        var paged = await _cartService.GetItemsAsync(cart.Id, pageNumber, pageSize);
        var result = new PagedResult<CartItemResponse>
        {
            Items = paged.Items.Select(ToItemResponse).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };

        return Ok(result);
    }

    [HttpGet("items/{id:guid}")]
    public async Task<IActionResult> GetItem(Guid id, [FromQuery] string? sessionId)
    {
        if (!TryGetCartOwner(sessionId, out var userId, out var resolvedSessionId))
            return BadRequest(new ApiResponse(false, "SessionId is required for guest carts"));

        var cart = await _cartService.GetCartAsync(userId, resolvedSessionId);
        if (cart == null) return NotFound(new ApiResponse(false, "Cart not found"));

        var item = await _cartService.GetItemByIdAsync(id, cart.Id);
        return item == null ? NotFound(new ApiResponse(false, "Cart item not found")) : Ok(ToItemResponse(item));
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] string? sessionId)
    {
        if (!TryGetCartOwner(sessionId, out var userId, out var resolvedSessionId))
            return BadRequest(new ApiResponse(false, "SessionId is required for guest carts"));

        var cart = await _cartService.GetOrCreateCartAsync(userId, resolvedSessionId);
        var items = await _cartService.GetItemsAsync(cart.Id);
        var subtotal = items.Sum(item => (item.Product?.Price ?? item.UnitPrice) * item.Quantity);

        bool? couponValid = null;
        string? couponError = null;
        var couponDiscount = 0m;

        if (!string.IsNullOrWhiteSpace(cart.CouponCode))
        {
            var offer = await _offerService.GetOfferByCodeAsync(cart.CouponCode);
            couponValid = TryCalculateCouponDiscount(offer, subtotal, DateTime.UtcNow, out couponDiscount, out couponError);
        }

        const decimal shippingCharges = 0m;
        var finalPayableAmount = Math.Max(0m, subtotal - couponDiscount) + shippingCharges;

        return Ok(new CartSummaryResponse
        {
            CartId = cart.Id,
            Subtotal = subtotal,
            CouponCode = cart.CouponCode,
            CouponValid = couponValid,
            CouponError = couponError,
            CouponDiscount = couponDiscount,
            ShippingCharges = shippingCharges,
            FinalPayableAmount = finalPayableAmount
        });
    }

    [HttpPost("apply-coupon")]
    public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request, [FromQuery] string? sessionId)
    {
        if (!TryGetCartOwner(sessionId, out var userId, out var resolvedSessionId))
            return BadRequest(new ApiResponse(false, "SessionId is required for guest carts"));
        if (string.IsNullOrWhiteSpace(request.CouponCode))
            return BadRequest(new ApiResponse(false, "CouponCode is required"));

        var cart = await _cartService.GetCartAsync(userId, resolvedSessionId);
        if (cart == null) return NotFound(new ApiResponse(false, "Cart not found"));

        var offer = await _offerService.GetOfferByCodeAsync(request.CouponCode.Trim());
        var items = await _cartService.GetItemsAsync(cart.Id);
        var subtotal = items.Sum(item => (item.Product?.Price ?? item.UnitPrice) * item.Quantity);
        if (!TryCalculateCouponDiscount(offer, subtotal, DateTime.UtcNow, out _, out var couponError))
            return BadRequest(new ApiResponse(false, couponError));

        cart.OfferId = offer!.Id;
        cart.CouponCode = offer.Code;
        cart.UpdatedAt = DateTime.UtcNow;
        await _cartService.UpdateCartAsync(cart);

        return await GetSummary(sessionId);
    }

    [HttpDelete("remove-coupon")]
    public async Task<IActionResult> RemoveCoupon([FromQuery] string? sessionId)
    {
        if (!TryGetCartOwner(sessionId, out var userId, out var resolvedSessionId))
            return BadRequest(new ApiResponse(false, "SessionId is required for guest carts"));

        var cart = await _cartService.GetCartAsync(userId, resolvedSessionId);
        if (cart == null) return NotFound(new ApiResponse(false, "Cart not found"));

        if (cart.OfferId.HasValue || !string.IsNullOrWhiteSpace(cart.CouponCode))
        {
            cart.OfferId = null;
            cart.CouponCode = null;
            cart.UpdatedAt = DateTime.UtcNow;
            await _cartService.UpdateCartAsync(cart);
        }

        return await GetSummary(sessionId);
    }

    private bool TryGetCartOwner(string? requestedSessionId, out Guid? userId, out string? sessionId)
    {
        userId = ClaimsHelper.GetUserIdFromClaims(User);
        sessionId = userId.HasValue ? null : requestedSessionId;
        return userId.HasValue || !string.IsNullOrWhiteSpace(sessionId);
    }

    private static object ToCartResponse(CartEntity cart) => new
    {
        CartId = cart.Id,
        cart.CouponCode,
        cart.CreatedAt,
        cart.UpdatedAt
    };

    private static CartItemResponse ToItemResponse(CartItem item) => new()
    {
        Id = item.Id,
        ProductId = item.ProductId,
        ProductName = item.Product?.Name ?? string.Empty,
        PrimaryImageId = item.Product?.Images?.FirstOrDefault(image => image.IsPrimary && image.MediaId.HasValue)?.MediaId,
        Quantity = item.Quantity,
        UnitPrice = item.Product?.Price ?? item.UnitPrice,
        StockQuantity = item.Product?.StockQuantity ?? 0
    };

    private static bool TryCalculateCouponDiscount(Offer? offer, decimal subtotal, DateTime now, out decimal discount, out string? error)
    {
        discount = 0m;
        error = null;

        if (offer == null || !offer.IsActive || offer.IsDeleted || offer.StartDate > now || offer.EndDate < now)
        {
            error = "The applied coupon is invalid, inactive, or expired.";
            return false;
        }

        if (offer.UsageLimit.HasValue && offer.UsageCount >= offer.UsageLimit.Value)
        {
            error = "The applied coupon has reached its usage limit.";
            return false;
        }

        if (offer.MinimumOrderAmount.HasValue && subtotal < offer.MinimumOrderAmount.Value)
        {
            error = $"The minimum order amount for this coupon is {offer.MinimumOrderAmount.Value:F2}.";
            return false;
        }

        var normalizedType = offer.DiscountType.Replace(" ", string.Empty).Trim().ToLowerInvariant();
        var calculatedDiscount = normalizedType switch
        {
            "percentage" or "percent" => subtotal * offer.DiscountValue / 100m,
            "fixed" or "fixedamount" or "amount" => offer.DiscountValue,
            _ => -1m
        };

        if (calculatedDiscount < 0m)
        {
            error = "The applied coupon has an unsupported discount type.";
            return false;
        }

        if (offer.MaximumDiscountAmount.HasValue)
            calculatedDiscount = Math.Min(calculatedDiscount, offer.MaximumDiscountAmount.Value);

        discount = Math.Clamp(calculatedDiscount, 0m, subtotal);
        return true;
    }
}
