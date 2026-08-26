using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.ProductFaqs.Interfaces;

public interface IProductFaqService
{
    Task<ProductFaq> AddFaqAsync(ProductFaq faq);
    Task UpdateFaqAsync(ProductFaq faq);
    Task DeleteFaqAsync(Guid id);
    Task<ProductFaq?> GetFaqByIdAsync(Guid id);
    Task<List<ProductFaq>> GetFaqsByProductAsync(Guid productId);
}
