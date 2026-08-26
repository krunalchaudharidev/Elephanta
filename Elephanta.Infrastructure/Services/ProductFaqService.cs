using Elephanta.Application.Features.ProductFaqs.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Services;

public class ProductFaqService : IProductFaqService
{
    private readonly ElephantaDbContext _db;

    public ProductFaqService(ElephantaDbContext db)
    {
        _db = db;
    }

    public async Task<ProductFaq> AddFaqAsync(ProductFaq faq)
    {
        _db.ProductFaqs.Add(faq);
        await _db.SaveChangesAsync();
        return faq;
    }

    public async Task UpdateFaqAsync(ProductFaq faq)
    {
        var existing = await _db.ProductFaqs.FindAsync(faq.Id);
        if (existing == null) throw new InvalidOperationException("FAQ not found");
        existing.Question = faq.Question;
        existing.Answer = faq.Answer;
        existing.IsActive = faq.IsActive;
        existing.DisplayOrder = faq.DisplayOrder;
        existing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteFaqAsync(Guid id)
    {
        var existing = await _db.ProductFaqs.FindAsync(id);
        if (existing == null) return;
        _db.ProductFaqs.Remove(existing);
        await _db.SaveChangesAsync();
    }

    public async Task<ProductFaq?> GetFaqByIdAsync(Guid id)
    {
        return await _db.ProductFaqs.Include(f => f.Product).FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<List<ProductFaq>> GetFaqsByProductAsync(Guid productId)
    {
        return await _db.ProductFaqs.Where(f => f.ProductId == productId).OrderBy(f => f.DisplayOrder).ToListAsync();
    }
}
