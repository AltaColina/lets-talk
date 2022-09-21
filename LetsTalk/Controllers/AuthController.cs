using LetsTalk.Dtos.Auths;
using LetsTalk.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody, Required] RegisterRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody, Required] LoginRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody, Required] RefreshRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}