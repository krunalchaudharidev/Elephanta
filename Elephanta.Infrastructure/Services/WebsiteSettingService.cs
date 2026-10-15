using Elephanta.Application.Features.WebsiteSettings.DTOs;
using Elephanta.Application.Features.WebsiteSettings.Interfaces;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Services;

public class WebsiteSettingService : IWebsiteSettingService
{
    private readonly IWebsiteSettingRepository _repo;

    public WebsiteSettingService(IWebsiteSettingRepository repo)
    {
        _repo = repo;
    }

    public async Task<WebsiteSettingDto?> GetAsync()
    {
        var s = await _repo.GetAsync();
        if (s == null) return null;

        return MapToDto(s);
    }

    public async Task<WebsiteSettingDto> SaveAsync(WebsiteSettingRequest request)
    {
        var existing = await _repo.GetAsync();
        if (existing == null)
        {
            existing = new WebsiteSetting
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(existing);
        }

        existing.SiteName = request.SiteName ?? string.Empty;
        existing.FacebookURL = request.FacebookURL;
        existing.InstagramURL = request.InstagramURL;
        existing.PhoneNo = request.PhoneNo;
        existing.Email = request.Email;
        existing.FreeShippingOnOrdersAbove = request.FreeShippingOnOrdersAbove;
        existing.Address = request.Address;
        existing.IsMaintenanceMode = request.IsMaintenanceMode;
        existing.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(existing);

        return MapToDto(existing)!;
    }

    private WebsiteSettingDto MapToDto(WebsiteSetting s)
    {
        return new WebsiteSettingDto
        {
            Id = s.Id,
            SiteName = s.SiteName,
            FacebookURL = s.FacebookURL,
            InstagramURL = s.InstagramURL,
            PhoneNo = s.PhoneNo,
            Email = s.Email,
            FreeShippingOnOrdersAbove = s.FreeShippingOnOrdersAbove,
            Address = s.Address,
            IsMaintenanceMode = s.IsMaintenanceMode,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };
    }
}
