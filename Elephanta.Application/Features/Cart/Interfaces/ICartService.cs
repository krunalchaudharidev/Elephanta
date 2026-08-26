using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Cart.Interfaces;

public interface ICartService
{
    Task<CartItem> AddItemAsync(CartItem item);
    Task UpdateItemAsync(CartItem item);
    Task DeleteItemAsync(Guid id);
    Task<CartItem?> GetItemByIdAsync(Guid id);
    Task<List<CartItem>> GetItemsByUserAsync(Guid userId);
    Task<Elephanta.Application.Common.PagedResult<CartItem>> GetItemsByUserAsync(Guid userId, int pageNumber, int pageSize);
}
