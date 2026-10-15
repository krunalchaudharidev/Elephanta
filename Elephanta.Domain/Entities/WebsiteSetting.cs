using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class WebsiteSetting : BaseEntity
{
    public string SiteName { get; set; } = string.Empty;

    public string? FacebookURL { get; set; }

    public string? InstagramURL { get; set; }

    public string? PhoneNo { get; set; }

    public string? Email { get; set; }

    public decimal FreeShippingOnOrdersAbove { get; set; }

    public string? Address { get; set; }

    public bool IsMaintenanceMode { get; set; }
}
