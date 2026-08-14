using System;

namespace Elephanta.Domain.Entities;

// Join entity for many-to-many between User and Role
public class UserRole
{
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public Guid RoleId { get; set; }

    public Role? Role { get; set; }
}
