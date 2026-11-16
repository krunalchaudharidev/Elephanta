using Elephanta.Application.Common;
using Elephanta.Application.Features.CommonFaqs.DTOs;
using Elephanta.Application.Features.CommonFaqs.Interfaces;
using Elephanta.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommonFaqsController : ControllerBase
{
    private readonly ICommonFaqService _service;

    public CommonFaqsController(ICommonFaqService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var faqs = await _service.GetAllAsync();
        return Ok(faqs);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var faq = await _service.GetAsync(id);
        if (faq == null)
        {
            return NotFound();
        }

        return Ok(faq);
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CommonFaqRequest request)
    {
        var faq = await _service.AddAsync(request);
        return CreatedAtAction(nameof(Get), new { id = faq.Id }, faq);
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CommonFaqRequest request)
    {
        var faq = await _service.UpdateAsync(id, request);
        if (faq == null)
        {
            return NotFound(new ApiResponse(false, "Common FAQ not found"));
        }

        return Ok(new ApiResponse(true, "Common FAQ updated successfully", faq.Id));
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ApiExplorerSettings(GroupName = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(new ApiResponse(false, "Common FAQ not found"));
        }

        return Ok(new ApiResponse(true, "Common FAQ deleted successfully", id));
    }
}
