using LetsTalk.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefreshController : ControllerBase
{
    private IMediator _mediator;

    public RefreshController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Refresh([FromBody, Required] RefreshRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}