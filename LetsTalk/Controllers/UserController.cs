using LetsTalk.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly ISender _mediator;

    public UserController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var response = await _mediator.Send(new UserGetRequest());
        return Ok(response);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> Get([FromRoute, Required] string userId)
    {
        var response = await _mediator.Send(new UserGetRequest { UserId = userId });
        return Ok(response);
    }
}
