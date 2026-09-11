using System;
using System.Threading.Tasks;
using Elephanta.Application.Features.Media.Interfaces;
using DomainMedia = Elephanta.Domain.Entities.Media;
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

    public async Task<DomainMedia> AddAsync(DomainMedia media)
    {
        _db.Medias.Add(media);
        await _db.SaveChangesAsync();
        return media;
    }

    public async Task<DomainMedia?> GetByIdAsync(Guid id)
    {
        return await _db.Medias.FirstOrDefaultAsync(m => m.Id == id);
    }
}
