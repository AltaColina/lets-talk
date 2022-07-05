using LetsTalk.Interfaces;
using LetsTalk.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet, Authorize(Permissions.Role.View)]
    public async Task<IActionResult> Get()
    {
        var response = await _mediator.Send(new RoleGetRequest());
        return Ok(response);
    }

    [HttpGet("{roleId}"), Authorize(Permissions.Role.View)]
    public async Task<IActionResult> Get([FromRoute, Required] string roleId)
    {
        var response = await _mediator.Send(new RoleGetRequest { RoleId = roleId });
        return Ok(response);
    }

    [HttpPost, Authorize(Permissions.Role.Create)]
    public async Task<IActionResult> Post([FromBody, Required] RolePostRequest request)
    {
        await _mediator.Send(request);
        return Ok();
    }

    [HttpPut, Authorize(Permissions.Role.Edit)]
    public async Task<IActionResult> Put([FromBody, Required] Role role)
    {
        await _mediator.Send(new RolePutRequest { Role = role });
        return Ok();
    }

    [HttpDelete("{roleId}"), Authorize(Permissions.Role.Delete)]
    public async Task<IActionResult> Delete([FromRoute, Required] string roleId)
    {
        await _mediator.Send(new RoleDeleteRequest { RoleId = roleId });
        return Ok();
    }
}
