using Elephanta.Application.Common;
using Elephanta.Application.Features.Cart.Interfaces;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _repository;

    public CartService(ICartRepository repository)
    {
        _repository = repository;
    }

    public Task<CartItem> AddItemAsync(CartItem item) => _repository.AddItemAsync(item);

    public Task UpdateItemAsync(CartItem item) => _repository.UpdateItemAsync(item);

    public Task DeleteItemAsync(Guid id) => _repository.DeleteItemAsync(id);

    public Task<CartItem?> GetItemByIdAsync(Guid id) => _repository.GetItemByIdAsync(id);

    public Task<List<CartItem>> GetItemsAsync(Guid? userId, string? sessionId)
        => _repository.GetItemsAsync(userId, sessionId);

    public Task<PagedResult<CartItem>> GetItemsAsync(Guid? userId, string? sessionId, int pageNumber, int pageSize)
        => _repository.GetItemsAsync(userId, sessionId, pageNumber, pageSize);
}
