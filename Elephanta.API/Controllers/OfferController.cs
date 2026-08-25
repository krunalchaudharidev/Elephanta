using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Elephanta.Domain.Constants;
using Elephanta.Application.Features.Offers.DTOs;
using Elephanta.Application.Features.Offers.Interfaces;
using Elephanta.Domain.Entities;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OfferController : ControllerBase
{
    private readonly IOfferService _service;

    public OfferController(IOfferService service)
    {
        _service = service;
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPost("offers")]
    public async Task<IActionResult> AddOffer([FromBody] OfferRequest req)
    {
        var o = new Offer
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Code = req.Code,
            Description = req.Description,
            DiscountType = req.DiscountType,
            DiscountValue = req.DiscountValue,
            MinimumOrderAmount = req.MinimumOrderAmount,
            MaximumDiscountAmount = req.MaximumDiscountAmount,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            UsageLimit = req.UsageLimit,
            IsActive = req.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _service.AddOfferAsync(o);
        var resp = new OfferResponse
        {
            Id = added.Id,
            Name = added.Name,
            Code = added.Code,
            Description = added.Description,
            DiscountType = added.DiscountType,
            DiscountValue = added.DiscountValue,
            MinimumOrderAmount = added.MinimumOrderAmount,
            MaximumDiscountAmount = added.MaximumDiscountAmount,
            StartDate = added.StartDate,
            EndDate = added.EndDate,
            UsageLimit = added.UsageLimit,
            UsageCount = added.UsageCount,
            IsActive = added.IsActive
        };

        return CreatedAtAction(nameof(GetOffer), new { id = resp.Id }, resp);
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPut("offers/{id}")]
    public async Task<IActionResult> UpdateOffer(Guid id, [FromBody] OfferRequest req)
    {
        var existing = await _service.GetOfferByIdAsync(id);
        if (existing == null) return NotFound();

        existing.Name = req.Name;
        existing.Code = req.Code;
        existing.Description = req.Description;
        existing.DiscountType = req.DiscountType;
        existing.DiscountValue = req.DiscountValue;
        existing.MinimumOrderAmount = req.MinimumOrderAmount;
        existing.MaximumDiscountAmount = req.MaximumDiscountAmount;
        existing.StartDate = req.StartDate;
        existing.EndDate = req.EndDate;
        existing.UsageLimit = req.UsageLimit;
        existing.IsActive = req.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _service.UpdateOfferAsync(existing);
        return NoContent();
    }

    [HttpGet("offers")]
    public async Task<IActionResult> GetOffers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetOffersAsync(pageNumber, pageSize);
        var items = paged.Items.Select(o => new OfferResponse
        {
            Id = o.Id,
            Name = o.Name,
            Code = o.Code,
            Description = o.Description,
            DiscountType = o.DiscountType,
            DiscountValue = o.DiscountValue,
            MinimumOrderAmount = o.MinimumOrderAmount,
            MaximumDiscountAmount = o.MaximumDiscountAmount,
            StartDate = o.StartDate,
            EndDate = o.EndDate,
            UsageLimit = o.UsageLimit,
            UsageCount = o.UsageCount,
            IsActive = o.IsActive
        }).ToList();

        var result = new Elephanta.Application.Common.PagedResult<OfferResponse>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };

        return Ok(result);
    }

    [HttpGet("offers/{id}")]
    public async Task<IActionResult> GetOffer(Guid id)
    {
        var o = await _service.GetOfferByIdAsync(id);
        if (o == null) return NotFound();
        var resp = new OfferResponse
        {
            Id = o.Id,
            Name = o.Name,
            Code = o.Code,
            Description = o.Description,
            DiscountType = o.DiscountType,
            DiscountValue = o.DiscountValue,
            MinimumOrderAmount = o.MinimumOrderAmount,
            MaximumDiscountAmount = o.MaximumDiscountAmount,
            StartDate = o.StartDate,
            EndDate = o.EndDate,
            UsageLimit = o.UsageLimit,
            UsageCount = o.UsageCount,
            IsActive = o.IsActive
        };
        return Ok(resp);
    }

    // Images
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPost("offers/{offerId}/images")]
    public async Task<IActionResult> AddImage(Guid offerId, [FromBody] OfferImageRequest req)
    {
        var img = new OfferImage
        {
            Id = Guid.NewGuid(),
            OfferId = offerId,
            ImageUrl = req.ImageUrl,
            IsPrimary = req.IsPrimary,
            DisplayOrder = req.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _service.AddImageAsync(img);
        var resp = new OfferImageResponse { Id = added.Id, ImageUrl = added.ImageUrl, IsPrimary = added.IsPrimary, DisplayOrder = added.DisplayOrder };
        return CreatedAtAction(nameof(GetImage), new { id = resp.Id }, resp);
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPut("offers/images/{id}")]
    public async Task<IActionResult> UpdateImage(Guid id, [FromBody] OfferImageRequest req)
    {
        var existing = await _service.GetImageByIdAsync(id);
        if (existing == null) return NotFound();
        existing.ImageUrl = req.ImageUrl;
        existing.IsPrimary = req.IsPrimary;
        existing.DisplayOrder = req.DisplayOrder;
        existing.UpdatedAt = DateTime.UtcNow;
        await _service.UpdateImageAsync(existing);
        return NoContent();
    }

    [HttpGet("offers/{offerId}/images")]
    public async Task<IActionResult> GetImages(Guid offerId)
    {
        var list = await _service.GetImagesByOfferAsync(offerId);
        var resp = list.Select(i => new OfferImageResponse { Id = i.Id, ImageUrl = i.ImageUrl, IsPrimary = i.IsPrimary, DisplayOrder = i.DisplayOrder }).ToList();
        return Ok(resp);
    }

    [HttpGet("offers/images/{id}")]
    public async Task<IActionResult> GetImage(Guid id)
    {
        var i = await _service.GetImageByIdAsync(id);
        if (i == null) return NotFound();
        var resp = new OfferImageResponse { Id = i.Id, ImageUrl = i.ImageUrl, IsPrimary = i.IsPrimary, DisplayOrder = i.DisplayOrder };
        return Ok(resp);
    }
}
