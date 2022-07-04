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

    [HttpGet]
    [Authorize]
    [Route("{chatId}")]
    public async Task<IActionResult> GetById([FromRoute] string? chatId)
    {
        var request = !String.IsNullOrWhiteSpace(chatId)
            ? new ChatGetRequest { ChatId = chatId }
            : new ChatGetRequest();
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetByName([FromQuery] string? chatName)
    {
        var request = !String.IsNullOrWhiteSpace(chatName)
            ? new ChatGetRequest { ChatName = chatName }
            : new ChatGetRequest();
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost]
    [Authorize("Administrators")]
    public async Task<IActionResult> Post([FromBody, Required] ChatPostRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPut]
    [Authorize("Administrators")]
    public async Task<IActionResult> Put([FromBody, Required] ChatPutRequest request)
    {
        await _mediator.Send(request);
        return Ok();
    }

    [HttpDelete]
    [Route("{chatId}")]
    [Authorize("Administrators")]
    public async Task<IActionResult> Delete([FromRoute, Required] string chatId)
    {
        var request = new ChatDeleteRequest { ChatId = chatId };
        await _mediator.Send(request);
        return Ok();
    }
}