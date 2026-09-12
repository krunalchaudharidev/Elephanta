using System;
using System.Threading.Tasks;
using Elephanta.Application.Features.Media.DTOs;

namespace Elephanta.Application.Features.Media.Interfaces;

public interface IMediaService
{
    Task<MediaDto> SaveAsync(ImageUploadDto dto);

    Task<(MediaDto?, Stream?)> GetFileAsync(Guid id);
}
