using LetsTalk.Commands.Users;
using LetsTalk.Models;
using LetsTalk.Queries.Chats;
using LetsTalk.Queries.Users;
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
    public async Task<IActionResult> Get()
    {
        var response = await _mediator.Send(new GetUsersRequest());
        return Ok(response);
    }

    [HttpGet("{userId}", Name = "GetUserById"), Authorize(Permissions.User.Read)]
    public async Task<IActionResult> Get([FromRoute, Required] string userId)
    {
        var response = await _mediator.Send(new GetUserByIdRequest { UserId = userId });
        return Ok(response);
    }

    [HttpPut("{userId}"), Authorize(Permissions.User.Update)]
    public async Task<IActionResult> Put([FromRoute, Required] string userId, [FromBody, Required] UpdateUserRequest user)
    {
        user.Id = userId;
        var response = await _mediator.Send(user);
        return Ok(response);
    }

    [HttpDelete("{userId}"), Authorize(Permissions.User.Delete)]
    public async Task<IActionResult> Delete([FromRoute, Required] string userId)
    {
        await _mediator.Send(new DeleteUserRequest { Id = userId });
        return NoContent();
    }

    [HttpGet("{userId}/chat"), Authorize(Permissions.User.Chat.Read)]
    public async Task<IActionResult> GetChats([FromRoute, Required] string userId)
    {
        var response = await _mediator.Send(new GetUserChatsRequest { UserId = userId });
        return Ok(response);
    }
}
