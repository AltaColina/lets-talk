using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GreetController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var name = HttpContext.User.GetDisplayName();
        if (String.IsNullOrWhiteSpace(name))
            name = "Guest";
        return await Task.FromResult(Ok($"Hello, {name}!"));
    }
}
