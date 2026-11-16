using Elephanta.Application.Features.CommonFaqs.DTOs;
using Elephanta.Application.Features.CommonFaqs.Interfaces;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Services;

public class CommonFaqService : ICommonFaqService
{
    private readonly ICommonFaqRepository _repository;

    public CommonFaqService(ICommonFaqRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CommonFaqResponse>> GetAllAsync()
    {
        var faqs = await _repository.GetAllAsync();
        return faqs.Select(MapToResponse).ToList();
    }

    public async Task<CommonFaqResponse?> GetAsync(Guid id)
    {
        var faq = await _repository.GetByIdAsync(id);
        return faq == null ? null : MapToResponse(faq);
    }

    public async Task<CommonFaqResponse> AddAsync(CommonFaqRequest request)
    {
        var faq = new CommonFaq
        {
            Id = Guid.NewGuid(),
            Question = request.Question,
            Answer = request.Answer,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(faq);
        return MapToResponse(faq);
    }

    public async Task<CommonFaqResponse?> UpdateAsync(Guid id, CommonFaqRequest request)
    {
        var faq = await _repository.GetByIdAsync(id);
        if (faq == null)
        {
            return null;
        }

        faq.Question = request.Question;
        faq.Answer = request.Answer;
        faq.IsActive = request.IsActive;
        faq.DisplayOrder = request.DisplayOrder;
        faq.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(faq);
        return MapToResponse(faq);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var faq = await _repository.GetByIdAsync(id);
        if (faq == null)
        {
            return false;
        }

        await _repository.DeleteAsync(faq);
        return true;
    }

    private static CommonFaqResponse MapToResponse(CommonFaq faq)
    {
        return new CommonFaqResponse
        {
            Id = faq.Id,
            Question = faq.Question,
            Answer = faq.Answer,
            IsActive = faq.IsActive,
            DisplayOrder = faq.DisplayOrder,
            CreatedAt = faq.CreatedAt,
            UpdatedAt = faq.UpdatedAt
        };
    }
}
