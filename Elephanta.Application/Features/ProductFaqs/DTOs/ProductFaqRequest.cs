using System;

namespace Elephanta.Application.Features.ProductFaqs.DTOs;

public class ProductFaqRequest
{
    public string Question { get; set; } = null!;
    public string? Answer { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}
