using LetsTalk.Chats.Commands;
using LetsTalk.Chats.Queries;
using LetsTalk.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ISender _mediator;

    public ChatController(ISender mediator) => _mediator = mediator;

    [HttpGet, Authorize(Permissions.Chat.Read)]
    public async Task<IActionResult> Get()
    {
        var response = await _mediator.Send(new GetChatsQuery());
        return Ok(response);
    }

    [HttpGet("{chatId}", Name = "GetChatById"), Authorize(Permissions.Chat.Read)]
    public async Task<IActionResult> Get([FromRoute, Required] string chatId)
    {
        var response = await _mediator.Send(new GetChatByIdQuery { ChatId = chatId, });
        return Ok(response);
    }

    [HttpPost, Authorize(Permissions.Chat.Create)]
    public async Task<IActionResult> Post([FromBody, Required] CreateChatCommand chat)
    {
        var response = await _mediator.Send(chat);
        return CreatedAtRoute("GetChatById", new { chatId = response.Id }, response);
    }

    [HttpPut("{chatId}"), Authorize(Permissions.Chat.Update)]
    public async Task<IActionResult> Put([FromRoute, Required] string chatId, [FromBody, Required] UpdateChatCommand chat)
    {
        chat.Id = chatId;
        var response = await _mediator.Send(chat);
        return Ok(response);
    }

    [HttpDelete("{chatId}"), Authorize(Permissions.Chat.Delete)]
    public async Task<IActionResult> Delete([FromRoute, Required] string chatId)
    {
        var request = new DeleteChatCommand { Id = chatId };
        await _mediator.Send(request);
        return NoContent();
    }

    [HttpGet("{chatId}/user"), Authorize(Permissions.Chat.User.Read)]
    public async Task<IActionResult> GetUsers([FromRoute, Required] string chatId)
    {
        var response = await _mediator.Send(new GetChatUsersQuery { ChatId = chatId });
        return Ok(response);
    }
}