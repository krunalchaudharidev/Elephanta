using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Elephanta.Domain.Constants;
using Elephanta.Application.Features.Offers.DTOs;
using Elephanta.Application.Features.Offers.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Application.Common;

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
            StartDate = DateTimeConverter.ConvertIstToUtc(req.StartDate),
            EndDate = DateTimeConverter.ConvertIstToUtc(req.EndDate),
            UsageLimit = req.UsageLimit,
            IsActive = req.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _service.AddOfferAsync(o);
        // If request contains ImageIds, create OfferImage records
        if (req.ImageIds != null && req.ImageIds.Count > 0)
        {
            int order = 0;
            foreach (var imgId in req.ImageIds)
            {
                var oi = new OfferImage
                {
                    Id = Guid.NewGuid(),
                    OfferId = added.Id,
                    MediaId = imgId,
                    IsPrimary = (req.PrimaryImageId.HasValue && req.PrimaryImageId.Value == imgId) || order == 0,
                    DisplayOrder = order,
                    CreatedAt = DateTime.UtcNow
                };

                await _service.AddImageAsync(oi);
                order++;
            }
        }
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
            IsActive = added.IsActive,
            ImageIds = (req.ImageIds != null && req.ImageIds.Count > 0) ? req.ImageIds : (added.Images?.Where(i => i.MediaId.HasValue).Select(i => i.MediaId!.Value).ToList() ?? new List<Guid>()),
            PrimaryImageId = req.PrimaryImageId.HasValue ? req.PrimaryImageId : added.Images?.FirstOrDefault(i => i.IsPrimary && i.MediaId.HasValue)?.MediaId
        };

        return CreatedAtAction(nameof(GetOffer), new { id = resp.Id }, resp);
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPut("offers/{id}")]
    public async Task<IActionResult> UpdateOffer(Guid id, [FromBody] OfferUpdateRequest req)
    {
        var existing = await _service.GetOfferByIdAsync(id);
        if (existing == null) return NotFound(new ApiResponse(false, "Offer not found"));

        // Apply partial updates for scalar fields
        if (req.Name != null) existing.Name = req.Name;
        if (req.Code != null) existing.Code = req.Code;
        if (req.Description != null) existing.Description = req.Description;
        if (req.DiscountType != null) existing.DiscountType = req.DiscountType;
        if (req.DiscountValue.HasValue) existing.DiscountValue = req.DiscountValue.Value;
        if (req.MinimumOrderAmount.HasValue) existing.MinimumOrderAmount = req.MinimumOrderAmount;
        if (req.MaximumDiscountAmount.HasValue) existing.MaximumDiscountAmount = req.MaximumDiscountAmount;
        if (req.StartDate.HasValue) existing.StartDate = DateTimeConverter.ConvertIstToUtc(req.StartDate.Value);
        if (req.EndDate.HasValue) existing.EndDate = DateTimeConverter.ConvertIstToUtc(req.EndDate.Value);
        if (req.UsageLimit.HasValue) existing.UsageLimit = req.UsageLimit.Value;
        if (req.IsActive.HasValue) existing.IsActive = req.IsActive.Value;
        // Handle image additions: create OfferImage records linking this offer to media
        if (req.ImageIds != null && req.ImageIds.Count > 0)
        {
            var currentImages = await _service.GetImagesByOfferAsync(existing.Id);
            var order = currentImages?.Count ?? 0;
            var existingMediaIds = currentImages?.Where(i => i.MediaId.HasValue).Select(i => i.MediaId!.Value).ToHashSet() ?? new HashSet<Guid>();

            foreach (var mediaId in req.ImageIds.Distinct())
            {
                if (existingMediaIds.Contains(mediaId)) continue;

                var oi = new OfferImage
                {
                    Id = Guid.NewGuid(),
                    OfferId = existing.Id,
                    MediaId = mediaId,
                    IsPrimary = (req.PrimaryImageId.HasValue && req.PrimaryImageId.Value == mediaId) || order == 0,
                    DisplayOrder = order,
                    CreatedAt = DateTime.UtcNow
                };

                await _service.AddImageAsync(oi);
                existingMediaIds.Add(mediaId);
                order++;
            }

            // Handle primary image selection by media id
            if (req.PrimaryImageId.HasValue)
            {
                // First, unset any existing primary images for this offer
                var pImg = currentImages.FirstOrDefault(i => i.IsPrimary);
                if (pImg != null)
                {
                    pImg.IsPrimary = false;
                    pImg.UpdatedAt = DateTime.UtcNow;
                    await _service.UpdateImageAsync(pImg);
                }

                // Then set the selected image (by media id) as primary
                var img = await _service.GetImageByMediaIdAsync(req.PrimaryImageId.Value);
                if (img != null)
                {
                    img.IsPrimary = true;
                    img.UpdatedAt = DateTime.UtcNow;
                    await _service.UpdateImageAsync(img);
                }
            }
        }

        existing.UpdatedAt = DateTime.UtcNow;

        await _service.UpdateOfferAsync(existing);
        return Ok(new ApiResponse(true, "Offer updated successfully", existing.Id));
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
            IsActive = o.IsActive,
            ImageIds = o.Images?.Where(i => i.MediaId.HasValue).Select(i => i.MediaId!.Value).ToList() ?? new List<Guid>(),
            PrimaryImageId = o.Images?.FirstOrDefault(i => i.IsPrimary && i.MediaId.HasValue)?.MediaId
        }).ToList();

        var result = new PagedResult<OfferResponse>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };

        return Ok(result);
    }

    [HttpGet("offers/search")]
    public async Task<IActionResult> SearchOffers([FromQuery] string? name, [FromQuery] string? code, [FromQuery] bool? isActive, [FromQuery] string? sort, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.SearchOffersAsync(name, code, sort, isActive, pageNumber, pageSize);
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
            IsActive = o.IsActive,
            ImageIds = o.Images?.Where(i => i.MediaId.HasValue).Select(i => i.MediaId!.Value).ToList() ?? new List<Guid>(),
            PrimaryImageId = o.Images?.FirstOrDefault(i => i.IsPrimary && i.MediaId.HasValue)?.MediaId
        }).ToList();

        var result = new PagedResult<OfferResponse>
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
            IsActive = o.IsActive,
            ImageIds = o.Images?.Where(i => i.MediaId.HasValue).Select(i => i.MediaId!.Value).ToList() ?? new List<Guid>(),
            PrimaryImageId = o.Images?.FirstOrDefault(i => i.IsPrimary && i.MediaId.HasValue)?.MediaId
        };
        return Ok(resp);
    }

    /// <summary>
    /// Requires Admin role. Soft-delete an offer by setting IsDeleted and DeletedAt.
    /// </summary>
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpDelete("offers/{id}")]
    public async Task<IActionResult> DeleteOffer(Guid id)
    {
        var existing = await _service.GetOfferByIdAsync(id);
        if (existing == null) return NotFound(new ApiResponse(false, "Offer not found"));

        existing.IsDeleted = true;
        existing.DeletedAt = DateTime.UtcNow;
        existing.UpdatedAt = DateTime.UtcNow;

        await _service.UpdateOfferAsync(existing);

        return Ok(new ApiResponse(true, "Offer deleted successfully", id));
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
            MediaId = req.MediaId,
            IsPrimary = req.IsPrimary,
            DisplayOrder = req.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _service.AddImageAsync(img);
        var resp = new OfferImageResponse { Id = added.Id, MediaId = added.MediaId, IsPrimary = added.IsPrimary, DisplayOrder = added.DisplayOrder };
        return CreatedAtAction(nameof(GetImage), new { id = resp.Id }, resp);
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPut("offers/images/{id}")]
    public async Task<IActionResult> UpdateImage(Guid id, [FromBody] OfferImageRequest req)
    {
        var existing = await _service.GetImageByIdAsync(id);
        if (existing == null) return NotFound();
        existing.MediaId = req.MediaId;
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
        var resp = list.Select(i => new OfferImageResponse { Id = i.Id, MediaId = i.MediaId, IsPrimary = i.IsPrimary, DisplayOrder = i.DisplayOrder }).ToList();
        return Ok(resp);
    }

    [HttpGet("offers/images/{id}")]
    public async Task<IActionResult> GetImage(Guid id)
    {
        var i = await _service.GetImageByIdAsync(id);
        if (i == null) return NotFound();
        var resp = new OfferImageResponse { Id = i.Id, MediaId = i.MediaId, IsPrimary = i.IsPrimary, DisplayOrder = i.DisplayOrder };
        return Ok(resp);
    }
}
