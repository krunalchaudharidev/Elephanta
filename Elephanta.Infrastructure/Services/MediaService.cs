using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elephanta.Application.Features.Media.Interfaces;
using Elephanta.Application.Features.Media.DTOs;
using Elephanta.Domain.Entities;
using ImageMagick;

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
        var timestamp = DateTime.UtcNow.ToString("ddMMyyyyHHmmss");
        var safeModule = string.IsNullOrWhiteSpace(dto.ModuleType) ? "product" : dto.ModuleType.ToLowerInvariant();
        var fileName = $"{safeModule}_{timestamp}_{Guid.NewGuid()}{ext}";
        var relativePath = Path.Combine("uploads", folder, fileName);
        var fullPath = Path.Combine(_contentRoot, relativePath);

        // Save file (optionally compress). Use Magick.NET (ImageMagick) when compression is requested.
        if (dto.IsCompress)
        {
            dto.Content.Position = 0;

            // Read uploaded stream into memory first
            await using var inMs = new MemoryStream();
            await dto.Content.CopyToAsync(inMs);
            inMs.Position = 0;

            using var image = new MagickImage(inMs);

            var lower = dto.ContentType?.ToLower() ?? string.Empty;

            // Basic compression strategy: strip metadata, optionally reduce quality for JPEG,
            // and optimize PNG. You can tune Quality and other settings as needed.
            image.Strip();

            if (lower.Contains("png"))
            {
                // For PNG, strip metadata and write as PNG. For further optimization consider
                // using external tools (pngquant/oxipng) or Magick.NET advanced options.
                image.Format = MagickFormat.Png;
                image.Write(fullPath, MagickFormat.Png);
            }
            else
            {
                // For JPEG and others, set quality and write as JPEG where appropriate
                image.Quality = 75; // adjust quality as needed

                if (lower.Contains("jpeg") || lower.Contains("jpg"))
                {
                    image.Format = MagickFormat.Jpeg;
                    image.Write(fullPath, MagickFormat.Jpeg);
                }
                else
                {
                    // Fallback: write using original format
                    image.Write(fullPath);
                }
            }
        }
        else
        {
            using (var fs = File.Create(fullPath))
            {
                await dto.Content.CopyToAsync(fs);
            }
        }

        var fi = new FileInfo(fullPath);

        var media = new Media
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
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

    public async Task DeleteAsync(Guid id)
    {
        await _repo.DeleteByIdAsync(id);
    }
}
