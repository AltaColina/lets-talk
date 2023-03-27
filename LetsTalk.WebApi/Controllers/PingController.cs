using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PingController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Ping()
    {
        var name = HttpContext.User.FindFirst("name")?.Value ?? "Guest";
        return await Task.FromResult(Ok($"Hello, {name}!"));
    }
}
