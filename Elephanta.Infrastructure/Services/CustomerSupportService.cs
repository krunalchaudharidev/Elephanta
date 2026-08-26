using Elephanta.Application.Features.Support.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Services;

public class CustomerSupportService : ICustomerSupportService
{
    private readonly ElephantaDbContext _db;

    public CustomerSupportService(ElephantaDbContext db)
    {
        _db = db;
    }

    public async Task<CustomerSupportRequest> AddAsync(CustomerSupportRequest req)
    {
        _db.CustomerSupportRequests.Add(req);
        await _db.SaveChangesAsync();
        return req;
    }

    public async Task UpdateAsync(CustomerSupportRequest req)
    {
        var existing = await _db.CustomerSupportRequests.FindAsync(req.Id);
        if (existing == null) throw new InvalidOperationException("Support request not found");
        existing.Subject = req.Subject;
        existing.Message = req.Message;
        existing.Email = req.Email;
        existing.Phone = req.Phone;
        existing.IsResolved = req.IsResolved;
        existing.ResolvedAt = req.IsResolved ? DateTime.UtcNow : null;
        existing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _db.CustomerSupportRequests.FindAsync(id);
        if (existing == null) return;
        _db.CustomerSupportRequests.Remove(existing);
        await _db.SaveChangesAsync();
    }

    public async Task<CustomerSupportRequest?> GetByIdAsync(Guid id)
    {
        return await _db.CustomerSupportRequests.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<CustomerSupportRequest>> GetByUserAsync(Guid userId)
    {
        return await _db.CustomerSupportRequests.Where(c => c.UserId == userId).ToListAsync();
    }

    public async Task<Elephanta.Application.Common.PagedResult<CustomerSupportRequest>> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = _db.CustomerSupportRequests.AsQueryable();
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(c => c.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new Elephanta.Application.Common.PagedResult<CustomerSupportRequest>
        {
            Items = items,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
