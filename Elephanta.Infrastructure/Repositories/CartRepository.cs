using Elephanta.Application.Common;
using Elephanta.Application.Features.Cart.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Elephanta.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ElephantaDbContext _db;

    public CartRepository(ElephantaDbContext db)
    {
        _db = db;
    }

    public async Task<CartItem> AddItemAsync(CartItem item)
    {
        var existing = await _db.CartItems
            .FirstOrDefaultAsync(cartItem =>
                cartItem.ProductId == item.ProductId &&
                (item.UserId.HasValue
                    ? cartItem.UserId == item.UserId && cartItem.SessionId == null
                    : cartItem.UserId == null && cartItem.SessionId == item.SessionId));
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
        return await _db.CartItems
            .Include(cartItem => cartItem.Product)
                .ThenInclude(product => product.Images)
            .FirstOrDefaultAsync(cartItem => cartItem.Id == id);
    }

    public async Task<List<CartItem>> GetItemsAsync(Guid? userId, string? sessionId)
    {
        return await _db.CartItems
            .Where(cartItem => userId.HasValue
                ? cartItem.UserId == userId && cartItem.SessionId == null
                : cartItem.UserId == null && cartItem.SessionId == sessionId)
            .Include(cartItem => cartItem.Product)
                .ThenInclude(product => product.Images)
            .ToListAsync();
    }

    public async Task<PagedResult<CartItem>> GetItemsAsync(Guid? userId, string? sessionId, int pageNumber, int pageSize)
    {
        var query = _db.CartItems
            .Where(cartItem => userId.HasValue
                ? cartItem.UserId == userId && cartItem.SessionId == null
                : cartItem.UserId == null && cartItem.SessionId == sessionId)
            .Include(cartItem => cartItem.Product)
                .ThenInclude(product => product.Images)
            .AsQueryable();
        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<CartItem>
        {
            Items = items,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
