using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elephanta.Application.Features.Media.Interfaces;
using Elephanta.Application.Features.Media.DTOs;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Services;

public class MediaService : IMediaService
{
    private readonly IMediaRepository _repo;
    private readonly string _contentRoot;
    private readonly string[] _subFolders = new[] { "category", "offer", "product", "product-review", "user" };

    public MediaService(IMediaRepository repo)
    {
        _repo = repo;
        _contentRoot = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
    }

    public async Task<MediaDto> SaveAsync(ImageUploadDto dto)
    {
        var uploadsRoot = Path.Combine(_contentRoot, "uploads");
        if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);
        var folder = _subFolders.Contains(dto?.ModuleType?.ToLower()) ? dto.ModuleType.ToLower() : "product";
        var folderPath = Path.Combine(uploadsRoot, folder);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        var ext = Path.GetExtension(dto.FileName) ?? string.Empty;
        var fileName = $"{Guid.NewGuid()}{ext}";
        var relativePath = Path.Combine("uploads", folder, fileName);
        var fullPath = Path.Combine(_contentRoot, relativePath);

        // Save file
        using (var fs = File.Create(fullPath))
        {
            await dto.Content.CopyToAsync(fs);
        }

        var fi = new FileInfo(fullPath);

        var media = new Media
        {
            Id = Guid.NewGuid(),
            FileName = dto.FileName,
            FilePath = relativePath,
            FileType = dto.ContentType,
            FileSizeBytes = fi.Length,
            ModuleType = folder,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(media);
        return new MediaDto
        {
            Id = media.Id,
            FileName = media.FileName,
            FilePath = media.FilePath,
            FileType = media.FileType,
            FileSizeBytes = media.FileSizeBytes,
            ModuleType = media.ModuleType,
            CreatedAt = media.CreatedAt
        };
    }

    public async Task<(MediaDto?, Stream?)> GetFileAsync(Guid id)
    {
        var media = await _repo.GetByIdAsync(id);
        if (media == null) return (null, null);

        var fullPath = Path.Combine(_contentRoot, media.FilePath);
        if (!File.Exists(fullPath)) return (new MediaDto
        {
            Id = media.Id,
            FileName = media.FileName,
            FilePath = media.FilePath,
            FileType = media.FileType,
            FileSizeBytes = media.FileSizeBytes,
            ModuleType = media.ModuleType,
            CreatedAt = media.CreatedAt
        }, null);

        var fs = File.OpenRead(fullPath);
        var dto = new MediaDto
        {
            Id = media.Id,
            FileName = media.FileName,
            FilePath = media.FilePath,
            FileType = media.FileType,
            FileSizeBytes = media.FileSizeBytes,
            ModuleType = media.ModuleType,
            CreatedAt = media.CreatedAt
        };
        return (dto, fs);
    }
}
