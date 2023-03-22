using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PingController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Ping()
    {
        var name = HttpContext.User.FindFirst("name")?.Value ?? "Guest";
        return Ok($"Hello, {name}!");
    }
}
