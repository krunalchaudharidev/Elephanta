using System.Threading.Tasks;
using Elephanta.Domain.Entities;

namespace Elephanta.Application.Features.Authentication.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);

    Task<Role> AddAsync(Role role);
}
