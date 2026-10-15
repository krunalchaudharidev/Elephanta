using Elephanta.Application.Features.WebsiteSettings.DTOs;

namespace Elephanta.Application.Features.WebsiteSettings.Interfaces;

public interface IWebsiteSettingService
{
    Task<WebsiteSettingDto?> GetAsync();
    Task<WebsiteSettingDto> SaveAsync(WebsiteSettingRequest request);
}
