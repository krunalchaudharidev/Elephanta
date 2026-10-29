using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elephanta.Application.Common;
using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Cart.Interfaces;

public interface ICartRepository
{
    Task<CartItem> AddItemAsync(CartItem item);

    Task UpdateItemAsync(CartItem item);

    Task DeleteItemAsync(Guid id);

    Task<CartItem?> GetItemByIdAsync(Guid id);

    Task<List<CartItem>> GetItemsAsync(Guid? userId, string? sessionId);

    Task<PagedResult<CartItem>> GetItemsAsync(Guid? userId, string? sessionId, int pageNumber, int pageSize);
}
