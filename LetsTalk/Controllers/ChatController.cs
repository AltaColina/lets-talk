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
        var response = await _mediator.Send(new ChatGetRequest());
        return Ok(response);
    }

    [HttpGet("{chatId}"), Authorize(Permissions.Chat.View)]
    public async Task<IActionResult> Get([FromRoute, Required] string chatId)
    {
        var response = await _mediator.Send(new ChatGetRequest { ChatId = chatId, });
        return Ok(response);
    }

    [HttpPost, Authorize(Permissions.Chat.Create)]
    public async Task<IActionResult> Post([FromBody, Required] ChatPostRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPut, Authorize(Permissions.Chat.Edit)]
    public async Task<IActionResult> Put([FromBody, Required] ChatPutRequest request)
    {
        await _mediator.Send(request);
        return Ok();
    }

    [HttpDelete("{chatId}"), Authorize(Permissions.Chat.Delete)]
    public async Task<IActionResult> Delete([FromRoute, Required] string chatId)
    {
        var request = new ChatDeleteRequest { ChatId = chatId };
        await _mediator.Send(request);
        return Ok();
    }
}