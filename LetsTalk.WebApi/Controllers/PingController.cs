using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PingController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Ping()
    {
        var name = HttpContext.User.GetDisplayName() ?? "Guest";
        return await Task.FromResult(Ok($"Hello, {name}!"));
    }
}
