using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;

    // Navigation
    public ICollection<UserRole>? UserRoles { get; set; }
}
