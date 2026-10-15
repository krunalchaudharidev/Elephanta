using Elephanta.Application.Features.WebsiteSettings.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Repositories;

public class WebsiteSettingRepository : IWebsiteSettingRepository
{
    private readonly ElephantaDbContext _db;

    public WebsiteSettingRepository(ElephantaDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(WebsiteSetting setting)
    {
        _db.WebsiteSettings.Add(setting);
        await _db.SaveChangesAsync();
    }

    public async Task<WebsiteSetting?> GetAsync()
    {
        return await _db.WebsiteSettings.FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(WebsiteSetting setting)
    {
        _db.WebsiteSettings.Update(setting);
        await _db.SaveChangesAsync();
    }
}
