using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly ISender _mediator;

    public RoleController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> Get([FromRoute, Required] string userId)
    {
        var response = await _mediator.Send(new RoleGetRequest { UserId = userId });
        return Ok(response);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> Put([FromRoute, Required] string userId, [FromBody, Required] List<Role> roles)
    {
        await _mediator.Send(new RolePutRequest { UserId = userId, Roles = roles });
        return Ok();
    }
}
