using Elephanta.Application.Common;
using Elephanta.Application.Features.Support.DTOs;
using Elephanta.Application.Features.Support.Interfaces;
using Elephanta.Domain.Constants;
using Elephanta.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        var entity = new CustomerSupportRequest
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
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
            Name = added.Name,
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
        var email = GetUserEmail();
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var list = await _service.GetByEmailAsync(email);
        var resp = list.Select(c => new CustomerSupportRequestResponse
        {
            Id = c.Id,
            Name = c.Name,
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
            Name = c.Name,
            Subject = c.Subject,
            Message = c.Message,
            Email = c.Email,
            Phone = c.Phone,
            IsResolved = c.IsResolved,
            ResolvedAt = c.ResolvedAt,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        var result = new PagedResult<CustomerSupportRequestResponse>
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
        if (req == null) return NotFound(new ApiResponse(false, "Customer support request not found"));

        var isAdmin = User.IsInRole("Admin");
        var email = GetUserEmail();
        if (!isAdmin && (string.IsNullOrWhiteSpace(email)
            || !string.Equals(req.Email, email, StringComparison.OrdinalIgnoreCase))) return NotFound(new ApiResponse(false, "Customer support request not found"));

        var resp = new CustomerSupportRequestResponse
        {
            Id = req.Id,
            Name = req.Name,
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
        if (existing == null) return NotFound(new ApiResponse(false, "Customer support request not found"));

        var isAdmin = User.IsInRole("Admin");
        var email = GetUserEmail();
        if (!isAdmin && (string.IsNullOrWhiteSpace(email)
            || !string.Equals(existing.Email, email, StringComparison.OrdinalIgnoreCase))) return NotFound(new ApiResponse(false, "Customer support request not found"));

        if (req.Name is null && req.Subject is null && req.Message is null && req.Email is null && req.Phone is null
            && (!isAdmin || !req.IsResolved.HasValue)) return Ok(new ApiResponse(true, "No changes detected", id));

        if (req.Name is not null) existing.Name = req.Name;
        if (req.Subject is not null) existing.Subject = req.Subject;
        if (req.Message is not null) existing.Message = req.Message;
        if (req.Email is not null) existing.Email = req.Email;
        if (req.Phone is not null) existing.Phone = req.Phone;
        if (isAdmin && req.IsResolved.HasValue && existing.IsResolved != req.IsResolved.Value)
        {
            existing.IsResolved = req.IsResolved.Value;
            existing.ResolvedAt = req.IsResolved.Value ? DateTime.UtcNow : null;
        }

        existing.UpdatedAt = DateTime.UtcNow;
        await _service.UpdateAsync(existing);
        return Ok(new ApiResponse(true, "Customer support request updated successfully", id));
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpDelete("requests/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound(new ApiResponse(false, "Customer support request not found"));

        await _service.DeleteAsync(id);
        return Ok(new ApiResponse(true, "Customer support request deleted successfully", id));
    }

    private string? GetUserEmail() =>
        User.FindFirst("email")?.Value ?? User.FindFirst(ClaimTypes.Email)?.Value;
}
