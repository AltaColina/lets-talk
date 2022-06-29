using LetsTalk.Models;
using LetsTalk.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly ISender _mediator;

    public LoginController(ISender mediator) => _mediator = mediator;

    public async Task<IActionResult> Login([FromBody, Required] LoginRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}
