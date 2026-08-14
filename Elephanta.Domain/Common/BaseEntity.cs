using System;

namespace Elephanta.Domain.Common;


public abstract class BaseEntity
{
    public Guid Id { get; set; }


    public DateTime CreatedAt { get; set; }


    public DateTime? UpdatedAt { get; set; }
}
