using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Elephanta.Application.Features.Authentication.DTOs;
using Elephanta.Application.Features.Authentication.Interfaces;
using Elephanta.Application.Features.Authentication.Services;
using Elephanta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        try
        {
            var resp = await _authService.RegisterAsync(req);
            return Ok(resp);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        try
        {
            var resp = await _authService.LoginAsync(req);
            return Ok(resp);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        try
        {
            var resp = await _authService.RefreshTokenAsync(req.RefreshToken);
            return Ok(resp);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest req)
    {
        await _authService.LogoutAsync(req.RefreshToken);
        return NoContent();
    }
}
