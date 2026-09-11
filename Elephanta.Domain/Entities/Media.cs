using System;
using Elephanta.Domain.Common;

namespace Elephanta.Domain.Entities;

public class Media : BaseEntity
{
    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public long FileSizeBytes { get; set; }

    public string ModuleType { get; set; } = null!; // e.g., "Product", "Category", etc.
}
