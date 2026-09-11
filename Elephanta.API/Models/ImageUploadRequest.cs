using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Elephanta.API.Models;

public class ImageUploadRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;

    public string ModuleType { get; set; } = "product";
}
