using System;

namespace Elephanta.Application.Features.Media.DTOs;

public class MediaDto
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public string ModuleType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
