using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LetsTalk.Exceptions;
using LetsTalk.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;

namespace LetsTalk.Services;

[Authorize]
public class LetsTalkService : LetsTalk.Models.LetsTalk.LetsTalkBase
{
    private static readonly Empty EmptyResponse = new();
    private readonly ISender _mediator;

    public LetsTalkService(ISender mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        return await _mediator.Send(request);
    }

    [AllowAnonymous]
    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        return await _mediator.Send(request);
    }

    public override async Task<Empty> Logout(LogoutRequest request, ServerCallContext context)
    {
        await _mediator.Send(request);
        return EmptyResponse;
    }

    [AllowAnonymous]
    public override async Task<RefreshResponse> Refresh(RefreshRequest request, ServerCallContext context)
    {
            return await _mediator.Send(request);
    }

    public override async Task Join(JoinRequest request, IServerStreamWriter<Message> responseStream, ServerCallContext context)
    {
        request.ResponseStream = responseStream;
        // This will block until client exits or chat is deleted.
        await _mediator.Send(request);
    }

    public override async Task<Empty> Leave(LeaveRequest request, ServerCallContext context)
    {
        await _mediator.Send(request);
        return EmptyResponse;
    }

    public override async Task<Empty> Send(Message request, ServerCallContext context)
    {
        await _mediator.Send(request);
        return EmptyResponse;
    }

    public override async Task<GetChatResponse> GetChat(GetChatRequest request, ServerCallContext context)
    {
        return await _mediator.Send(request);
    }

    [Authorize(Policy = "Administrator")]
    public override async Task<PostChatResponse> PostChat(PostChatRequest request, ServerCallContext context)
    {
        return await _mediator.Send(request);
    }

    [Authorize(Policy = "Administrator")]
    public override async Task<Empty> DeleteChat(DeleteChatRequest request, ServerCallContext context)
    {
        await _mediator.Send(request);
        return EmptyResponse;
    }
}
