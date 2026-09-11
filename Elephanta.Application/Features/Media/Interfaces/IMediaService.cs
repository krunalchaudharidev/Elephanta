using System;
using System.IO;
using System.Threading.Tasks;
namespace Elephanta.Application.Features.Media.Interfaces;

public interface IMediaService
{
    Task<Elephanta.Domain.Entities.Media> SaveAsync(System.IO.Stream content, string originalFileName, string contentType, string moduleType);

    Task<(Elephanta.Domain.Entities.Media?, System.IO.Stream?)> GetFileAsync(Guid id);
}
