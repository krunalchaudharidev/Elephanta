using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elephanta.Application.Features.Media.Interfaces;
using DomainMedia = Elephanta.Domain.Entities.Media;

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

    public async Task<DomainMedia> SaveAsync(Stream content, string originalFileName, string contentType, string moduleType)
    {
        var uploadsRoot = Path.Combine(_contentRoot, "uploads");
        if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

        var folder = _subFolders.Contains(moduleType?.ToLower()) ? moduleType.ToLower() : "product";
        var folderPath = Path.Combine(uploadsRoot, folder);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var ext = Path.GetExtension(originalFileName) ?? string.Empty;
        var fileName = $"{Guid.NewGuid()}{ext}";
        var relativePath = Path.Combine("uploads", folder, fileName);
        var fullPath = Path.Combine(_contentRoot, relativePath);

        // Save file
        using (var fs = File.Create(fullPath))
        {
            await content.CopyToAsync(fs);
        }

        var fi = new FileInfo(fullPath);

        var media = new DomainMedia
        {
            Id = Guid.NewGuid(),
            FileName = originalFileName,
            FilePath = relativePath,
            FileType = contentType,
            FileSizeBytes = fi.Length,
            ModuleType = folder,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(media);
        return media;
    }

    public async Task<(DomainMedia?, Stream?)> GetFileAsync(Guid id)
    {
        var media = await _repo.GetByIdAsync(id);
        if (media == null) return (null, null);

        var fullPath = Path.Combine(_contentRoot, media.FilePath);
        if (!File.Exists(fullPath)) return (media, null);

        var fs = File.OpenRead(fullPath);
        return (media, fs);
    }
}
