using Elephanta.Application.Common;
using Elephanta.Application.Features.Cart.Interfaces;
using Elephanta.Domain.Entities;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using CartEntity = Elephanta.Domain.Entities.Cart;

namespace Elephanta.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ElephantaDbContext _db;

    public CartRepository(ElephantaDbContext db)
    {
        _db = db;
    }

    public Task<CartEntity?> GetCartAsync(Guid? userId, string? sessionId)
    {
        ValidateCartOwner(userId, sessionId);
        return FindCartAsync(userId, sessionId);
    }

    public async Task<CartEntity> GetOrCreateCartAsync(Guid? userId, string? sessionId)
    {
        ValidateCartOwner(userId, sessionId);
        var existing = await FindCartAsync(userId, sessionId);
        if (existing != null) return existing;

        var cart = new CartEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SessionId = userId.HasValue ? null : sessionId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Carts.Add(cart);
        try
        {
            await _db.SaveChangesAsync();
            return cart;
        }
        catch (DbUpdateException)
        {
            _db.Entry(cart).State = EntityState.Detached;
            existing = await FindCartAsync(userId, sessionId);
            if (existing == null) throw;
            return existing;
        }
    }

    public async Task UpdateCartAsync(CartEntity cart)
    {
        _db.Carts.Update(cart);
        await _db.SaveChangesAsync();
    }

    public async Task<CartItem> AddItemToCartAsync(CartItem item)
    {
        var existing = await _db.CartItems.FirstOrDefaultAsync(cartItem =>
            cartItem.CartId == item.CartId && cartItem.ProductId == item.ProductId);

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

    public async Task<CartItem?> GetItemByProductIdAsync(Guid productId, Guid cartId)
    {
        return await _db.CartItems
            .Include(cartItem => cartItem.Product)
                .ThenInclude(product => product.Images)
            .FirstOrDefaultAsync(cartItem => cartItem.ProductId == productId && cartItem.CartId == cartId);
    }

    public async Task<CartItem?> GetItemByIdAsync(Guid id, Guid cartId)
    {
        return await _db.CartItems
            .Include(cartItem => cartItem.Product)
                .ThenInclude(product => product.Images)
            .FirstOrDefaultAsync(cartItem => cartItem.Id == id && cartItem.CartId == cartId);
    }

    public async Task DeleteItemAsync(Guid id, Guid cartId)
    {
        var existing = await _db.CartItems.FirstOrDefaultAsync(item => item.Id == id && item.CartId == cartId);
        if (existing == null) return;

        _db.CartItems.Remove(existing);
        await _db.SaveChangesAsync();
    }

    public async Task<List<CartItem>> GetItemsAsync(Guid cartId)
    {
        return await _db.CartItems
            .Where(item => item.CartId == cartId)
            .Include(item => item.Product)
                .ThenInclude(product => product.Images)
            .ToListAsync();
    }

    public async Task<PagedResult<CartItem>> GetItemsAsync(Guid cartId, int pageNumber, int pageSize)
    {
        var query = _db.CartItems
            .Where(item => item.CartId == cartId)
            .Include(item => item.Product)
                .ThenInclude(product => product.Images)
            .AsQueryable();

        var total = await query.CountAsync();
        var items = await query.OrderBy(item => item.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<CartItem>
        {
            Items = items,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private Task<CartEntity?> FindCartAsync(Guid? userId, string? sessionId)
    {
        return _db.Carts.FirstOrDefaultAsync(cart => userId.HasValue
            ? cart.UserId == userId && cart.SessionId == null
            : cart.UserId == null && cart.SessionId == sessionId);
    }

    private static void ValidateCartOwner(Guid? userId, string? sessionId)
    {
        if (userId.HasValue == !string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentException("Exactly one of UserId or SessionId must be provided.");
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

}
