using System;

namespace Elephanta.Application.Features.WebsiteSettings.DTOs;

public class WebsiteSettingDto
{
    public Guid Id { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string? FacebookURL { get; set; }
    public string? InstagramURL { get; set; }
    public string? PhoneNo { get; set; }
    public string? Email { get; set; }
    public decimal FreeShippingOnOrdersAbove { get; set; }
    public string? Address { get; set; }
    public bool IsMaintenanceMode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
