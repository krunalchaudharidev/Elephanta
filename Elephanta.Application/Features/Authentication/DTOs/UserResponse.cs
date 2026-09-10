using System;
using System.Collections.Generic;

namespace Elephanta.Application.Features.Authentication.DTOs;

public class UserResponse
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public List<string> Roles { get; set; } = new();
}
