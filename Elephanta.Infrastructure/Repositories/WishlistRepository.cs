using Elephanta.Application.Features.Wishlist.Interfaces;
using Elephanta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using WishlistEntity = Elephanta.Domain.Entities.Wishlist;

namespace Elephanta.Infrastructure.Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly ElephantaDbContext _db;

    public WishlistRepository(ElephantaDbContext db)
    {
        _db = db;
    }

    public async Task<List<WishlistEntity>> GetByUserIdAsync(Guid userId)
    {
        return await _db.Wishlists
            .Where(item => item.UserId == userId)
            .Include(item => item.Product)
            .ThenInclude(product => product.Images)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync();
    }

    public async Task<WishlistEntity?> AddAsync(Guid userId, Guid productId)
    {
        var existing = await _db.Wishlists
            .Include(item => item.Product)
            .ThenInclude(product => product.Images)
            .FirstOrDefaultAsync(item => item.UserId == userId && item.ProductId == productId);
        if (existing != null) return existing;

        var product = await _db.Products
            .Include(item => item.Images)
            .FirstOrDefaultAsync(item => item.Id == productId && item.IsActive && !item.IsDeleted);
        if (product == null) return null;

        var wishlist = new WishlistEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = productId,
            Product = product,
            CreatedAt = DateTime.UtcNow
        };

        _db.Wishlists.Add(wishlist);
        await _db.SaveChangesAsync();
        return wishlist;
    }

    public async Task<bool> RemoveAsync(Guid userId, Guid productId)
    {
        var wishlist = await _db.Wishlists
            .FirstOrDefaultAsync(item => item.UserId == userId && item.ProductId == productId);
        if (wishlist == null) return false;

        _db.Wishlists.Remove(wishlist);
        await _db.SaveChangesAsync();
        return true;
    }
}
