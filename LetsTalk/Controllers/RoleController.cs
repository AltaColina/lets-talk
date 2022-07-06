using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Models.Roles;
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
    public async Task<IActionResult> Post([FromBody, Required] Role role)
    {
        await _mediator.Send(new RolePostRequest { Role = role });
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

    [HttpGet("{roleId}/user"), Authorize(Permissions.Role.User.View)]
    public async Task<IActionResult> GetUsers([FromRoute, Required] string roleId)
    {
        var response = await _mediator.Send(new RoleUserGetRequest { RoleId = roleId });
        return Ok(response);
    }

    [HttpPut("{roleId}/user"), Authorize(Permissions.Role.User.Edit)]
    public async Task<IActionResult> PutUsers([FromRoute, Required] string roleId, [FromBody, Required] string userId)
    {
        await _mediator.Send(new RoleUserPutRequest { RoleId = roleId, UserId = userId });
        return Ok();
    }
}
