using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Elephanta.Domain.Constants;
using Elephanta.Application.Features.Support.DTOs;
using Elephanta.Application.Features.Support.Interfaces;
using Elephanta.Domain.Entities;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerSupportController : ControllerBase
{
    private readonly ICustomerSupportService _service;

    public CustomerSupportController(ICustomerSupportService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpPost("requests")]
    public async Task<IActionResult> Create([FromBody] CreateCustomerSupportRequest req)
    {
        Guid? userId = null;
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!string.IsNullOrEmpty(sub) && Guid.TryParse(sub, out var parsed)) userId = parsed;

        var entity = new CustomerSupportRequest
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Subject = req.Subject,
            Message = req.Message,
            Email = req.Email,
            Phone = req.Phone,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _service.AddAsync(entity);

        var resp = new CustomerSupportRequestResponse
        {
            Id = added.Id,
            UserId = added.UserId,
            Subject = added.Subject,
            Message = added.Message,
            Email = added.Email,
            Phone = added.Phone,
            IsResolved = added.IsResolved,
            ResolvedAt = added.ResolvedAt,
            CreatedAt = added.CreatedAt,
            UpdatedAt = added.UpdatedAt
        };

        return CreatedAtAction(nameof(GetRequest), new { id = resp.Id }, resp);
    }

    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpGet("requests/my")]
    public async Task<IActionResult> GetMyRequests()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId)) return Unauthorized();

        var list = await _service.GetByUserAsync(userId);
        var resp = list.Select(c => new CustomerSupportRequestResponse
        {
            Id = c.Id,
            UserId = c.UserId,
            Subject = c.Subject,
            Message = c.Message,
            Email = c.Email,
            Phone = c.Phone,
            IsResolved = c.IsResolved,
            ResolvedAt = c.ResolvedAt,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        return Ok(resp);
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpGet("requests")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetAllAsync(pageNumber, pageSize);
        var items = paged.Items.Select(c => new CustomerSupportRequestResponse
        {
            Id = c.Id,
            UserId = c.UserId,
            Subject = c.Subject,
            Message = c.Message,
            Email = c.Email,
            Phone = c.Phone,
            IsResolved = c.IsResolved,
            ResolvedAt = c.ResolvedAt,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        var result = new Elephanta.Application.Common.PagedResult<CustomerSupportRequestResponse>
        {
            Items = items,
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };

        return Ok(result);
    }

    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpGet("requests/{id}")]
    public async Task<IActionResult> GetRequest(Guid id)
    {
        var req = await _service.GetByIdAsync(id);
        if (req == null) return NotFound();

        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!string.IsNullOrEmpty(sub) && Guid.TryParse(sub, out var userId))
        {
            // if not admin, ensure ownership
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && req.UserId != userId) return NotFound();
        }

        var resp = new CustomerSupportRequestResponse
        {
            Id = req.Id,
            UserId = req.UserId,
            Subject = req.Subject,
            Message = req.Message,
            Email = req.Email,
            Phone = req.Phone,
            IsResolved = req.IsResolved,
            ResolvedAt = req.ResolvedAt,
            CreatedAt = req.CreatedAt,
            UpdatedAt = req.UpdatedAt
        };

        return Ok(resp);
    }

    [Authorize(Policy = AuthorizationPolicies.UserOrAdmin)]
    [HttpPut("requests/{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerSupportRequest req)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();

        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var isAdmin = User.IsInRole("Admin");

        if (!string.IsNullOrEmpty(sub) && Guid.TryParse(sub, out var userId))
        {
            if (!isAdmin && existing.UserId != userId) return NotFound();
        }
        else
        {
            return Unauthorized();
        }

        existing.Subject = req.Subject;
        existing.Message = req.Message;
        existing.Email = req.Email;
        existing.Phone = req.Phone;
        if (isAdmin && req.IsResolved.HasValue)
        {
            existing.IsResolved = req.IsResolved.Value;
            existing.ResolvedAt = req.IsResolved.Value ? DateTime.UtcNow : null;
        }

        existing.UpdatedAt = DateTime.UtcNow;
        await _service.UpdateAsync(existing);
        return NoContent();
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpDelete("requests/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();

        await _service.DeleteAsync(id);
        return NoContent();
    }
}
