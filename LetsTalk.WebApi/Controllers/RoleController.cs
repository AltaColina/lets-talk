using LetsTalk.Roles.Commands;
using LetsTalk.Roles.Queries;
using LetsTalk.Security;
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

    [HttpGet, Authorize(Permissions.Role.Read)]
    public async Task<IActionResult> Get()
    {
        var response = await _mediator.Send(new GetRolesQuery());
        return Ok(response);
    }

    [HttpGet("{roleId}", Name = "GetRoleById"), Authorize(Permissions.Role.Read)]
    public async Task<IActionResult> Get([FromRoute, Required] string roleId)
    {
        var response = await _mediator.Send(new GetRoleByIdQuery { RoleId = roleId });
        return Ok(response);
    }

    [HttpPost, Authorize(Permissions.Role.Create)]
    public async Task<IActionResult> Post([FromBody, Required] CreateRoleCommand role)
    {
        var response = await _mediator.Send(role);
        return CreatedAtRoute("GetRoleById", new { roleId = response.Id }, response);
    }

    [HttpPut("{roleId}"), Authorize(Permissions.Role.Update)]
    public async Task<IActionResult> Put([FromRoute, Required] string roleId, [FromBody, Required] UpdateRoleCommand role)
    {
        role.Id = roleId;
        var response = await _mediator.Send(role);
        return Ok(response);
    }

    [HttpDelete("{roleId}"), Authorize(Permissions.Role.Delete)]
    public async Task<IActionResult> Delete([FromRoute, Required] string roleId)
    {
        await _mediator.Send(new DeleteRoleCommand { Id = roleId });
        return NoContent();
    }

    [HttpGet("{roleId}/user"), Authorize(Permissions.Role.User.Read)]
    public async Task<IActionResult> GetUsers([FromRoute, Required] string roleId)
    {
        var response = await _mediator.Send(new GetRoleUsersQuery { Id = roleId });
        return Ok(response);
    }
}
