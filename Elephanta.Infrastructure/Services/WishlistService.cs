using Elephanta.Application.Features.Wishlist.Interfaces;
using WishlistEntity = Elephanta.Domain.Entities.Wishlist;

namespace Elephanta.Infrastructure.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _repository;

    public WishlistService(IWishlistRepository repository)
    {
        _repository = repository;
    }

    public Task<List<WishlistEntity>> GetByUserIdAsync(Guid userId) => _repository.GetByUserIdAsync(userId);

    public Task<WishlistEntity?> AddAsync(Guid userId, Guid productId) => _repository.AddAsync(userId, productId);

    public Task<bool> RemoveAsync(Guid userId, Guid productId) => _repository.RemoveAsync(userId, productId);
}
