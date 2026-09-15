using System;
using System.Threading.Tasks;
namespace Elephanta.Application.Features.Media.Interfaces;

public interface IMediaRepository
{
    Task<Elephanta.Domain.Entities.Media> AddAsync(Elephanta.Domain.Entities.Media media);

    Task<Elephanta.Domain.Entities.Media?> GetByIdAsync(Guid id);
    Task DeleteByIdAsync(Guid id);
}
