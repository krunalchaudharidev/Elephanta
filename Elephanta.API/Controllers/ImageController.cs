using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Elephanta.Application.Features.Media.Interfaces;
using Elephanta.Application.Features.Media.DTOs;
using Elephanta.API.Models;
using Elephanta.Domain.Constants;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public ImageController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] ImageUploadRequest req)
    {
        var file = req?.File;
        var moduleType = req?.ModuleType ?? "product";
        if (file == null || file.Length == 0) return BadRequest("No file uploaded");

        using var stream = file.OpenReadStream();
        var dto = new ImageUploadDto { Content = stream, FileName = file.FileName, ContentType = file.ContentType ?? "application/octet-stream", ModuleType = moduleType };
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
