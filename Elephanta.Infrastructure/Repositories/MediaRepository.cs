using System;
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
}
