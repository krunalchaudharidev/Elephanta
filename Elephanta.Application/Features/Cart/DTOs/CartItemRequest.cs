using System;

namespace Elephanta.Application.Features.Cart.DTOs;

public class CartItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
