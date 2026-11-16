using Elephanta.Application.Features.CommonFaqs.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Repositories;

public class CommonFaqRepository : ICommonFaqRepository
{
    private readonly ElephantaDbContext _db;

    public CommonFaqRepository(ElephantaDbContext db)
    {
        _db = db;
    }

    public async Task<List<CommonFaq>> GetAllAsync()
    {
        return await _db.CommonFaqs
            .AsNoTracking()
            .OrderBy(faq => faq.DisplayOrder)
            .ToListAsync();
    }

    public async Task<CommonFaq?> GetByIdAsync(Guid id)
    {
        return await _db.CommonFaqs
            .AsNoTracking()
            .FirstOrDefaultAsync(faq => faq.Id == id);
    }

    public async Task AddAsync(CommonFaq faq)
    {
        _db.CommonFaqs.Add(faq);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(CommonFaq faq)
    {
        _db.CommonFaqs.Update(faq);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(CommonFaq faq)
    {
        _db.CommonFaqs.Remove(faq);
        await _db.SaveChangesAsync();
    }
}
