using System;
using System.IO;
using System.Threading.Tasks;
using Elephanta.Application.Features.Media.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly ElephantaDbContext _db;

    public MediaRepository(ElephantaDbContext db)
    {
        _db = db;
    }

    public async Task<Media> AddAsync(Media media)
    {
        _db.Medias.Add(media);
        await _db.SaveChangesAsync();
        return media;
    }

    public async Task<Media?> GetByIdAsync(Guid id)
    {
        return await _db.Medias.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var media = await _db.Medias.FirstOrDefaultAsync(m => m.Id == id);
        if (media == null) return;

        // Attempt to delete file on disk
        try
        {
            var contentRoot = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
            var fullPath = Path.Combine(contentRoot, media.FilePath ?? string.Empty);
            if (!string.IsNullOrWhiteSpace(media.FilePath) && File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch
        {
            // ignore file deletion errors; proceed to remove record
        }

        _db.Medias.Remove(media);
        await _db.SaveChangesAsync();
    }
}
