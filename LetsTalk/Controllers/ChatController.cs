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
    private IMediator _mediator;

    public ChatController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize]
    [Route("{chatId}")]
    public async Task<IActionResult> GetById([FromRoute] string? chatId)
    {
        var request = new GetChatRequest();
        if (!String.IsNullOrWhiteSpace(chatId))
            request.ChatId = chatId;
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetByName([FromQuery] string? chatName)
    {
        var request = new GetChatRequest();
        if (!String.IsNullOrWhiteSpace(chatName))
            request.ChatName = chatName;
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> Post([FromBody, Required] PostChatRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPut]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> Put([FromBody, Required] PutChatRequest request)
    {
        await _mediator.Send(request);
        return Ok();
    }

    [HttpDelete]
    [Route("{chatId}")]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> Delete([FromRoute, Required] string chatId)
    {
        var request = new DeleteChatRequest { ChatId = chatId };
        await _mediator.Send(request);
        return Ok();
    }
}