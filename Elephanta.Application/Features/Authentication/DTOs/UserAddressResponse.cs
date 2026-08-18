using System;

namespace Elephanta.Application.Features.Authentication.DTOs;

public class UserAddressResponse
{
    public Guid Id { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public bool IsPrimary { get; set; }
}
