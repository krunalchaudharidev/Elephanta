using Elephanta.Application.Features.Cart.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly ElephantaDbContext _db;

    public CartService(ElephantaDbContext db)
    {
        _db = db;
    }

    public async Task<CartItem> AddItemAsync(CartItem item)
    {
        var existing = await _db.CartItems.FirstOrDefaultAsync(c => c.UserId == item.UserId && c.ProductId == item.ProductId);
        if (existing != null)
        {
            existing.Quantity += item.Quantity;
            existing.UnitPrice = item.UnitPrice;
            existing.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return existing;
        }

        _db.CartItems.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task UpdateItemAsync(CartItem item)
    {
        var existing = await _db.CartItems.FindAsync(item.Id);
        if (existing == null) throw new InvalidOperationException("Cart item not found");
        existing.Quantity = item.Quantity;
        existing.UnitPrice = item.UnitPrice;
        existing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(Guid id)
    {
        var existing = await _db.CartItems.FindAsync(id);
        if (existing == null) return;
        _db.CartItems.Remove(existing);
        await _db.SaveChangesAsync();
    }

    public async Task<CartItem?> GetItemByIdAsync(Guid id)
    {
        return await _db.CartItems.Include(c => c.Product).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<CartItem>> GetItemsByUserAsync(Guid userId)
    {
        return await _db.CartItems.Where(c => c.UserId == userId).Include(c => c.Product).ToListAsync();
    }

    public async Task<Elephanta.Application.Common.PagedResult<CartItem>> GetItemsByUserAsync(Guid userId, int pageNumber, int pageSize)
    {
        var query = _db.CartItems.Where(c => c.UserId == userId).Include(c => c.Product).AsQueryable();
        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new Elephanta.Application.Common.PagedResult<CartItem>
        {
            Items = items,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
