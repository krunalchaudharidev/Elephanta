using Microsoft.AspNetCore.Mvc;
using Elephanta.Application.Features.Media.Interfaces;
using DomainMedia = Elephanta.Domain.Entities.Media;

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

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] Models.ImageUploadRequest req)
    {
        var file = req?.File;
        var moduleType = req?.ModuleType ?? "product";
        if (file == null || file.Length == 0) return BadRequest("No file uploaded");

        using var stream = file.OpenReadStream();
        var media = await _mediaService.SaveAsync(stream, file.FileName, file.ContentType, moduleType);

        return CreatedAtAction(nameof(Get), new { id = media.Id }, new { id = media.Id, path = media.FilePath });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var (media, stream) = await _mediaService.GetFileAsync(id);
        if (media == null) return NotFound();
        if (stream == null) return NotFound("File not found on disk");

        return File(stream, media.FileType ?? "application/octet-stream", media.FileName);
    }
}
