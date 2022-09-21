﻿using LetsTalk.Dtos.Chats;
using LetsTalk.Models;
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

    [HttpGet, Authorize(Permissions.Chat.View)]
    public async Task<IActionResult> Get()
    {
        var response = await _mediator.Send(new GetChatsRequest());
        return Ok(response);
    }

    [HttpGet("{chatId}"), Authorize(Permissions.Chat.View)]
    public async Task<IActionResult> Get([FromRoute, Required] string chatId)
    {
        var response = await _mediator.Send(new GetChatsRequest { Id = chatId, });
        return Ok(response.Chats[0]);
    }

    [HttpPost, Authorize(Permissions.Chat.Create)]
    public async Task<IActionResult> Post([FromBody, Required] CreateChatRequest chat)
    {
        await _mediator.Send(chat);
        return Ok();
    }

    [HttpPut, Authorize(Permissions.Chat.Edit)]
    public async Task<IActionResult> Put([FromRoute, Required] string chatId, [FromBody, Required] UpdateChatRequest chat)
    {
        chat.Id = chatId;
        await _mediator.Send(chat);
        return Ok();
    }

    [HttpDelete("{chatId}"), Authorize(Permissions.Chat.Delete)]
    public async Task<IActionResult> Delete([FromRoute, Required] string chatId)
    {
        var request = new DeleteChatRequest { Id = chatId };
        await _mediator.Send(request);
        return Ok();
    }

    [HttpGet("{chatId}/user"), Authorize(Permissions.Chat.User.View)]
    public async Task<IActionResult> GetUsers([FromRoute, Required] string chatId)
    {
        var response = await _mediator.Send(new GetChatUsersRequest { Id = chatId });
        return Ok(response);
    }
}