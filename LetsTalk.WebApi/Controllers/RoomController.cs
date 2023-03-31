using LetsTalk.Rooms.Commands;
using LetsTalk.Rooms.Queries;
using LetsTalk.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly ISender _mediator;

    public RoomController(ISender mediator) => _mediator = mediator;

    [HttpGet, Authorize(Permissions.Room.Read)]
    public async Task<IActionResult> Get()
    {
        var response = await _mediator.Send(new GetRoomsQuery());
        return Ok(response);
    }

    [HttpGet("{roomId}", Name = "GetRoomById"), Authorize(Permissions.Room.Read)]
    public async Task<IActionResult> Get([FromRoute, Required] string roomId)
    {
        var response = await _mediator.Send(new GetRoomByIdQuery { RoomId = roomId, });
        return response.ToActionResult(Ok, roomId);
    }

    [HttpPost, Authorize(Permissions.Room.Create)]
    public async Task<IActionResult> Post([FromBody, Required] CreateRoomCommand room)
    {
        var response = await _mediator.Send(room);
        return response.ToActionResult(room => CreatedAtRoute("GetRoomById", new { roomId = room.Id }, room), room.Name);
    }

    [HttpPut("{roomId}"), Authorize(Permissions.Room.Update)]
    public async Task<IActionResult> Put([FromRoute, Required] string roomId, [FromBody, Required] UpdateRoomCommand room)
    {
        room.Id = roomId;
        var response = await _mediator.Send(room);
        return response.ToActionResult(Ok, roomId);
    }

    [HttpDelete("{roomId}"), Authorize(Permissions.Room.Delete)]
    public async Task<IActionResult> Delete([FromRoute, Required] string roomId)
    {
        var response = await _mediator.Send(new DeleteRoomCommand { Id = roomId });
        return response.ToActionResult(success => NoContent(), roomId);
    }

    [HttpGet("{roomId}/user"), Authorize(Permissions.Room.User.Read)]
    public async Task<IActionResult> GetUsers([FromRoute, Required] string roomId)
    {
        var response = await _mediator.Send(new GetRoomUsersQuery { RoomId = roomId });
        return response.ToActionResult(Ok, roomId);
    }
}