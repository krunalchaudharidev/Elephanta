namespace Elephanta.Application.Features.Cart.DTOs;

public class CartSummaryResponse
{
    public Guid CartId { get; set; }
    public decimal Subtotal { get; set; }
    public string? CouponCode { get; set; }
    public bool? CouponValid { get; set; }
    public string? CouponError { get; set; }
    public decimal CouponDiscount { get; set; }
    public decimal ShippingCharges { get; set; }
    public decimal FinalPayableAmount { get; set; }
}
