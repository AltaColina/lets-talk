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
    public async Task<IActionResult> Get([FromQuery] List<string?>? permission)
    {
        var response = await _mediator.Send(new GetRolesQuery { Permissions = permission });
        return Ok(response);
    }

    [HttpGet("{roleId}", Name = "GetRoleById"), Authorize(Permissions.Role.Read)]
    public async Task<IActionResult> Get([FromRoute, Required] string roleId)
    {
        var response = await _mediator.Send(new GetRoleByIdQuery { RoleId = roleId });
        return response.ToActionResult(Ok, roleId);
    }

    [HttpPost, Authorize(Permissions.Role.Create)]
    public async Task<IActionResult> Post([FromBody, Required] CreateRoleCommand role)
    {
        var response = await _mediator.Send(role);
        return response.ToActionResult(role => CreatedAtRoute("GetRoleById", new { roleId = role.Id }, role), role.Name);
    }

    [HttpPut("{roleId}"), Authorize(Permissions.Role.Update)]
    public async Task<IActionResult> Put([FromRoute, Required] string roleId, [FromBody, Required] UpdateRoleCommand role)
    {
        role.Id = roleId;
        var response = await _mediator.Send(role);
        return response.ToActionResult(Ok, roleId);
    }

    [HttpDelete("{roleId}"), Authorize(Permissions.Role.Delete)]
    public async Task<IActionResult> Delete([FromRoute, Required] string roleId)
    {
        var response = await _mediator.Send(new DeleteRoleCommand { Id = roleId });
        return response.ToActionResult(success => NoContent(), roleId);
    }
}
