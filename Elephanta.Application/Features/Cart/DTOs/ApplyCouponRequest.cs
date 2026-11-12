using System.ComponentModel.DataAnnotations;

namespace Elephanta.Application.Features.Cart.DTOs;

public class ApplyCouponRequest
{
    [Required]
    [StringLength(100)]
    public string CouponCode { get; set; } = string.Empty;
}
