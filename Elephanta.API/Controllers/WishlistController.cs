using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Elephanta.API.Helpers;
using Elephanta.Application.Common;
using Elephanta.Application.Features.Wishlist.DTOs;
using Elephanta.Application.Features.Wishlist.Interfaces;
using Elephanta.Domain.Constants;
using Elephanta.Domain.Entities;
using WishlistEntity = Elephanta.Domain.Entities.Wishlist;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
[ApiExplorerSettings(GroupName = "User")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        var userId = ClaimsHelper.GetUserIdFromClaims(User);
        if (!userId.HasValue) return Unauthorized();

        var items = await _wishlistService.GetByUserIdAsync(userId.Value);
        return Ok(items.Select(ToResponse).ToList());
    }

    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddWishlistRequest request)
    {
        var userId = ClaimsHelper.GetUserIdFromClaims(User);
        if (!userId.HasValue) return Unauthorized();

        var item = await _wishlistService.AddAsync(userId.Value, request.ProductId);
        if (item == null) return NotFound(new ApiResponse(false, "Product not found or unavailable"));

        return Ok(ToResponse(item));
    }

    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Remove(Guid productId)
    {
        var userId = ClaimsHelper.GetUserIdFromClaims(User);
        if (!userId.HasValue) return Unauthorized();

        await _wishlistService.RemoveAsync(userId.Value, productId);
        return Ok(new ApiResponse(true, "Wishlist item deleted successfully", productId));
    }

    private static WishlistResponse ToResponse(WishlistEntity item)
    {
        var images = item.Product == null
            ? new List<ProductImage>()
            : item.Product.Images
                .Where(image => image.MediaId.HasValue)
                .OrderBy(image => image.DisplayOrder)
                .ToList();

        return new WishlistResponse
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product?.Name ?? string.Empty,
            Price = item.Product?.Price ?? 0,
            ImageIds = images.Select(image => image.MediaId!.Value).ToList()
        };
    }
}
