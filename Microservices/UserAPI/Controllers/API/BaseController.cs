using Microsoft.AspNetCore.Mvc;

namespace UserAPI.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult InternalServerError()
    {
        return StatusCode(500);
    }
}