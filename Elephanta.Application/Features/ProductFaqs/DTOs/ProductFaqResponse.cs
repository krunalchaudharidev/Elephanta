using System;

namespace Elephanta.Application.Features.ProductFaqs.DTOs;

public class ProductFaqResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Question { get; set; } = null!;
    public string? Answer { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}
