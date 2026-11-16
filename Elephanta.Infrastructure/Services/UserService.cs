using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Application.Common;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResult<User>> GetCustomersAsync(
        int pageNumber,
        int pageSize,
        string? name,
        string? email,
        string? phoneNumber,
        bool sortDescending)
    {
        return await _userRepository.GetCustomersAsync(
            pageNumber,
            pageSize,
            name,
            email,
            phoneNumber,
            sortDescending);
    }

    public async Task<User> CreateAsync(User user)
    {
        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task UpdateAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
    }
}
