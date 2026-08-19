using System;
using System.Threading.Tasks;
using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Domain.Entities;

namespace Elephanta.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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
