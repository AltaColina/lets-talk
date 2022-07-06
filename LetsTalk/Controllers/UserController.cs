using LetsTalk.Models;
using LetsTalk.Models.Users;
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

    [HttpGet, Authorize(Permissions.User.View)]
    public async Task<IActionResult> Get()
    {
        var response = await _mediator.Send(new UserGetRequest());
        return Ok(response);
    }

    [HttpGet("{userId}"), Authorize(Permissions.User.View)]
    public async Task<IActionResult> Get([FromRoute, Required] string userId)
    {
        var response = await _mediator.Send(new UserGetRequest { UserId = userId });
        return Ok(response);
    }

    [HttpPost, Authorize(Permissions.User.Create)]
    public async Task<IActionResult> Post([FromBody, Required] User user)
    {
        await _mediator.Send(new UserPostRequest { User = user });
        return Ok();
    }

    [HttpPut, Authorize(Permissions.User.Edit)]
    public async Task<IActionResult> Put([FromBody, Required] User user)
    {
        await _mediator.Send(new UserPutRequest { User = user });
        return Ok();
    }

    [HttpDelete("{userId}"), Authorize(Permissions.User.Delete)]
    public async Task<IActionResult> Delete([FromRoute, Required] string userId)
    {
        await _mediator.Send(new UserDeleteRequest { UserId = userId });
        return Ok();
    }

    [HttpGet("{userId}/role"), Authorize(Permissions.User.Role.View)]
    public async Task<IActionResult> GetRoles([FromRoute, Required] string userId)
    {
        var response = await _mediator.Send(new UserRoleGetRequest { UserId = userId });
        return Ok(response);
    }

    [HttpPut("{userId}/role"), Authorize(Permissions.User.Role.Edit)]
    public async Task<IActionResult> PutRoles([FromRoute, Required] string userId, [FromBody, Required] UserRolePutRequest request)
    {
        var response = await _mediator.Send(new UserRolePutRequest { UserId = userId, Roles = request.Roles });
        return Ok();
    }

    [HttpGet("{userId}/chat"), Authorize(Permissions.User.Chat.View)]
    public async Task<IActionResult> GetChats([FromRoute, Required] string userId)
    {
        var response = await _mediator.Send(new UserChatGetRequest { UserId = userId });
        return Ok(response);
    }

    [HttpPut("{userId}/chat"), Authorize(Permissions.User.Chat.Edit)]
    public async Task<IActionResult> PutChats([FromRoute, Required] string userId, [FromBody, Required] UserChatPutRequest request)
    {
        var response = await _mediator.Send(new UserChatPutRequest { UserId = userId, Chats = request.Chats });
        return Ok();
    }
}
