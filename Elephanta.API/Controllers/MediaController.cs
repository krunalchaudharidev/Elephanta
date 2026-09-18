using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Elephanta.Application.Features.Media.Interfaces;
using Elephanta.Application.Features.Media.DTOs;
using Elephanta.API.Models;
using Elephanta.Domain.Constants;
using Elephanta.API.Helpers;
using Elephanta.Application.Common;
using Elephanta.Application.Features.Catalog.Interfaces;
using Elephanta.Application.Features.Offers.Interfaces;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;
    private readonly IProductService _productService;
    private readonly IOfferService _offerService;

    public MediaController(IMediaService mediaService, IProductService productService, IOfferService offerService)
    {
        _mediaService = mediaService;
        _productService = productService;
        _offerService = offerService;
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpDelete("{id?}")]
    public async Task<IActionResult> Delete([FromRoute] Guid? id, [FromQuery] string? moduleType)
    {
        if (!id.HasValue || string.IsNullOrWhiteSpace(moduleType)) return BadRequest(new ApiResponse(false, "id and moduleType are required"));

        var mtype = moduleType.Trim().ToLowerInvariant();

        switch (mtype)
        {
            case "category":
            {
                // find category by media id
                var category = await _productService.GetCategoryByMediaIdAsync(id.Value);
                if (category == null) return NotFound(new ApiResponse(false, "Category not found for given media id"));
                // clear relation and update
                category.MediaId = null;
                await _productService.UpdateCategoryAsync(category);
                break;
            }
            case "offer":
            {
                // find offer image by media id
                var offerImg = await _offerService.GetImageByMediaIdAsync(id.Value);
                if (offerImg == null) return NotFound(new ApiResponse(false, "Offer image not found for given media id"));
                // capture image id then delete the offer image record
                await _offerService.DeleteImageAsync(offerImg.Id);
                break;
            }
            case "product":
            {
                // find product image by media id
                var prodImg = await _productService.GetImageByMediaIdAsync(id.Value);
                if (prodImg == null) return NotFound(new ApiResponse(false, "Product image not found for given media id"));
                await _productService.DeleteImageAsync(prodImg.Id);
                break;
            }
            default:
                return BadRequest(new ApiResponse(false, "Invalid moduleType"));
        }

        // finally delete media record and file
        await _mediaService.DeleteAsync(id.Value);

        return Ok(new ApiResponse(true, "Media relation and media deleted successfully"));
    }

    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] ImageUploadRequest req)
    {
        var file = req?.File;
        var moduleType = req?.ModuleType ?? "product";
        if (file == null || file.Length == 0) return BadRequest("No file uploaded");

        if (!MediaHelper.ValidateAllowed(moduleType ?? string.Empty, file.ContentType, file.FileName, out var detectedType, out var allowedTypes))
        {
            return BadRequest($"The '{moduleType}' module does not allow {detectedType.ToString().ToLower()} files.");
        }

        using var stream = file.OpenReadStream();
        var dto = new ImageUploadDto { Content = stream, FileName = file.FileName, ContentType = file.ContentType ?? "application/octet-stream", ModuleType = moduleType, IsCompress = req.IsCompress };
        MediaDto mediaDto = await _mediaService.SaveAsync(dto);

        return CreatedAtAction(nameof(Get), new { id = mediaDto.Id }, new { id = mediaDto.Id, path = mediaDto.FilePath });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var (mediaDto, stream) = await _mediaService.GetFileAsync(id);
        if (mediaDto == null) return NotFound();
        if (stream == null) return NotFound("File not found on disk");

        return File(stream, mediaDto.FileType ?? "application/octet-stream", mediaDto.FileName);
    }
}
