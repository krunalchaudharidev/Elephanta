using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Elephanta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    // This endpoint is intentionally AllowAnonymous so it requires no authentication or permissions.
    [AllowAnonymous]
    [HttpGet("hello")]
    public IActionResult GetHello()
    {
        return Ok(new { message = "Hello from Elephanta API sample controller." });
    }
}
