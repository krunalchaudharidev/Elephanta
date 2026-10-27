using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WishlistEntity = Elephanta.Domain.Entities.Wishlist;

namespace Elephanta.Application.Features.Wishlist.Interfaces;

public interface IWishlistService
{
    Task<List<WishlistEntity>> GetByUserIdAsync(Guid userId);

    Task<WishlistEntity?> AddAsync(Guid userId, Guid productId);

    Task<bool> RemoveAsync(Guid userId, Guid productId);
}
