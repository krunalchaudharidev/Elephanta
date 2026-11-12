using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elephanta.Application.Common;
using Elephanta.Domain.Entities;
using CartEntity = Elephanta.Domain.Entities.Cart;

namespace Elephanta.Application.Features.Cart.Interfaces;

public interface ICartRepository
{
    Task<CartEntity?> GetCartAsync(Guid? userId, string? sessionId);

    Task<CartEntity> GetOrCreateCartAsync(Guid? userId, string? sessionId);

    Task UpdateCartAsync(CartEntity cart);

    Task<CartItem> AddItemToCartAsync(CartItem item);

    Task<CartItem?> GetItemByProductIdAsync(Guid productId, Guid cartId);

    Task<CartItem?> GetItemByIdAsync(Guid id, Guid cartId);

    Task DeleteItemAsync(Guid id, Guid cartId);

    Task<List<CartItem>> GetItemsAsync(Guid cartId);

    Task<PagedResult<CartItem>> GetItemsAsync(Guid cartId, int pageNumber, int pageSize);

    Task UpdateItemAsync(CartItem item);
}
