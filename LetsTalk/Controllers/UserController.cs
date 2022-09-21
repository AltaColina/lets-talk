using LetsTalk.Dtos.Users;
using LetsTalk.Models;
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
        var response = await _mediator.Send(new GetUsersRequest());
        return Ok(response);
    }

    [HttpGet("{userId}"), Authorize(Permissions.User.View)]
    public async Task<IActionResult> Get([FromRoute, Required] string userId)
    {
        var response = await _mediator.Send(new GetUsersRequest { Id = userId });
        return Ok(response);
    }

    [HttpPut, Authorize(Permissions.User.Edit)]
    public async Task<IActionResult> Put([FromRoute, Required] string userId, [FromBody, Required] UpdateUserRequest user)
    {
        user.Id = userId;
        await _mediator.Send(user);
        return Ok();
    }

    [HttpDelete("{userId}"), Authorize(Permissions.User.Delete)]
    public async Task<IActionResult> Delete([FromRoute, Required] string userId)
    {
        await _mediator.Send(new DeleteUserRequest { Id = userId });
        return Ok();
    }
}
