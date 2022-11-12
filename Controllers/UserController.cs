namespace KinoAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
[Authorize]

public class UserController : ControllerBase
{
    public UserController() {}
    [HttpGet("AuthenticationHealthCheck")]
    public async Task<ActionResult> GetProtectedNumber()
    {
        return Ok($"Auth is working properly");
    }
}