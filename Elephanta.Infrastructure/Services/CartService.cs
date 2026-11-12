using Elephanta.Application.Common;
using Elephanta.Application.Features.Cart.Interfaces;
using Elephanta.Domain.Entities;
using CartEntity = Elephanta.Domain.Entities.Cart;

namespace Elephanta.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _repository;

    public CartService(ICartRepository repository)
    {
        _repository = repository;
    }

    public Task<CartEntity?> GetCartAsync(Guid? userId, string? sessionId)
        => _repository.GetCartAsync(userId, sessionId);

    public Task<CartEntity> GetOrCreateCartAsync(Guid? userId, string? sessionId)
        => _repository.GetOrCreateCartAsync(userId, sessionId);

    public Task UpdateCartAsync(CartEntity cart)
        => _repository.UpdateCartAsync(cart);

    public Task<CartItem> AddItemToCartAsync(CartItem item)
        => _repository.AddItemToCartAsync(item);

    public Task<CartItem?> GetItemByProductIdAsync(Guid productId, Guid cartId)
        => _repository.GetItemByProductIdAsync(productId, cartId);

    public Task<CartItem?> GetItemByIdAsync(Guid id, Guid cartId)
        => _repository.GetItemByIdAsync(id, cartId);

    public Task DeleteItemAsync(Guid id, Guid cartId)
        => _repository.DeleteItemAsync(id, cartId);

    public Task<List<CartItem>> GetItemsAsync(Guid cartId)
        => _repository.GetItemsAsync(cartId);

    public Task<PagedResult<CartItem>> GetItemsAsync(Guid cartId, int pageNumber, int pageSize)
        => _repository.GetItemsAsync(cartId, pageNumber, pageSize);

    public Task UpdateItemAsync(CartItem item) => _repository.UpdateItemAsync(item);
}
