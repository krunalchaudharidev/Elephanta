using Elephanta.Application.Common;
using Elephanta.Application.Features.WebsiteSettings.DTOs;
using Elephanta.Application.Features.WebsiteSettings.Interfaces;
using Elephanta.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebsiteSettingController : ControllerBase
{
    private readonly IWebsiteSettingService _service;

    public WebsiteSettingController(IWebsiteSettingService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var setting = await _service.GetAsync();
        if (setting == null) return NotFound();

        return Ok(setting);
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] WebsiteSettingRequest req)
    {
        if (req == null) return BadRequest();

        var saved = await _service.SaveAsync(req);

        return Ok(new ApiResponse(true, "Website setting updated successfully", saved.Id));
    }
}
