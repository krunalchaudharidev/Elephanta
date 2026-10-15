using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.WebsiteSettings.Interfaces;

public interface IWebsiteSettingRepository
{
    Task<WebsiteSetting?> GetAsync();
    Task AddAsync(WebsiteSetting setting);
    Task UpdateAsync(WebsiteSetting setting);
}
