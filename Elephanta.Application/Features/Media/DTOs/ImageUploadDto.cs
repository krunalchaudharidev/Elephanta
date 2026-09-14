using System.IO;

namespace Elephanta.Application.Features.Media.DTOs;

public class ImageUploadDto
{
    public Stream Content { get; set; } = null!;

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public string ModuleType { get; set; } = "product";
    public bool IsCompress { get; set; } = false;
}
