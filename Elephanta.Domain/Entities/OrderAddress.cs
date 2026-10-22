using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class OrderAddress : BaseEntity
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public string AddressType { get; set; } = null!;

    public string RecipientName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = null!;

    public string? State { get; set; }

    public string PostalCode { get; set; } = null!;

    public string Country { get; set; } = null!;
}
