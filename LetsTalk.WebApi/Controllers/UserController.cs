using LetsTalk.Security;
using LetsTalk.Users.Commands;
using LetsTalk.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet, Authorize(Permissions.User.Read)]
    public async Task<IActionResult> Get([FromQuery] List<string?>? role)
    {
        var response = await _mediator.Send(new GetUsersQuery { Roles = role });
        return Ok(response);
    }

    [HttpGet("{userId}", Name = "GetUserById"), Authorize(Permissions.User.Read)]
    public async Task<IActionResult> Get([FromRoute, Required] string userId)
    {
        var response = await _mediator.Send(new GetUserByIdQuery { UserId = userId });
        return Ok(response);
    }

    [HttpPut("{userId}"), Authorize(Permissions.User.Update)]
    public async Task<IActionResult> Put([FromRoute, Required] string userId, [FromBody, Required] UpdateUserCommand user)
    {
        user.Id = userId;
        var response = await _mediator.Send(user);
        return Ok(response);
    }

    [HttpDelete("{userId}"), Authorize(Permissions.User.Delete)]
    public async Task<IActionResult> Delete([FromRoute, Required] string userId)
    {
        await _mediator.Send(new DeleteUserCommand { Id = userId });
        return NoContent();
    }

    [HttpGet("{userId}/room"), Authorize(Permissions.User.Room.Read)]
    public async Task<IActionResult> GetRooms([FromRoute, Required] string userId)
    {
        var response = await _mediator.Send(new GetUserRoomsQuery { UserId = userId });
        return Ok(response);
    }
}
