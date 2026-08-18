using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class UserAddress : BaseEntity
{
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public bool IsPrimary { get; set; }
}
