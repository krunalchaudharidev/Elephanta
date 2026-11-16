using Elephanta.Application.Features.CommonFaqs.DTOs;

namespace Elephanta.Application.Features.CommonFaqs.Interfaces;

public interface ICommonFaqService
{
    Task<List<CommonFaqResponse>> GetAllAsync();

    Task<CommonFaqResponse?> GetAsync(Guid id);

    Task<CommonFaqResponse> AddAsync(CommonFaqRequest request);

    Task<CommonFaqResponse?> UpdateAsync(Guid id, CommonFaqRequest request);

    Task<bool> DeleteAsync(Guid id);
}
