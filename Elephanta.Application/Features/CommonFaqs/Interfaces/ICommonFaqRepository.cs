using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.CommonFaqs.Interfaces;

public interface ICommonFaqRepository
{
    Task<List<CommonFaq>> GetAllAsync();

    Task<CommonFaq?> GetByIdAsync(Guid id);

    Task AddAsync(CommonFaq faq);

    Task UpdateAsync(CommonFaq faq);

    Task DeleteAsync(CommonFaq faq);
}
