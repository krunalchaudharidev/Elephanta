using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Elephanta.Application.Features.Wishlist.DTOs;

public class AddWishlistRequest
{
    [Required]
    public Guid ProductId { get; set; }
}

public class WishlistResponse
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public List<Guid> ImageIds { get; set; } = new();
}
