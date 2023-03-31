using Duende.IdentityServer.Extensions;
using LetsTalk.Hubs.Commands;
using LetsTalk.Hubs.Queries;
using LetsTalk.Messaging;
using LetsTalk.Rooms.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.Hubs;

[Authorize]
public sealed class LetsTalkHub : Hub
{
    private readonly ISender _mediator;

    public LetsTalkHub(ISender mediator)
    {
        _mediator = mediator;
    }

    public override async Task OnConnectedAsync()
    {
        var response = await _mediator.Send(new ConnectCommand
        {
            ConnectionId = Context.ConnectionId,
            UserId = Context.User.GetSubjectId()
        });

        var user = response.EnsureSuccess();

        await Task.WhenAll(user.Rooms.Select(roomId => Groups.AddToGroupAsync(Context.ConnectionId, roomId)));
        await Clients.Others.SendAsync(new ConnectMessage
        {
            UserId = user.Id,
            UserName = user.Name,
            UserImage = user.Image,
        });
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var response = await _mediator.Send(new DisconnectCommand
        {
            ConnectionId = Context.ConnectionId,
            UserId = Context.User.GetSubjectId()
        });

        var user = response.EnsureSuccess();

        await Task.WhenAll(user.Rooms.Select(roomId => Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId)));
        await Clients.Others.SendAsync(new DisconnectMessage
        {
            UserId = user.Id,
            UserName = user.Name,
            UserImage = user.Image
        });
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<JoinRoomResponse> JoinRoomAsync(string roomId)
    {
        var response = await _mediator.Send(new JoinRoomCommand
        {
            RoomId = roomId,
            UserId = Context.User.GetSubjectId()
        });

        var join = response.EnsureSuccess();

        if (join.HasUserJoined)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(join.Room.Id).SendAsync(new JoinRoomMessage
            {
                RoomId = join.Room.Id,
                RoomName = join.Room.Name,
                UserId = join.User.Id,
                UserName = join.User.Name,
            });
        }

        return join;
    }

    public async Task<LeaveRoomResponse> LeaveRoomAsync(string roomId)
    {
        var response = await _mediator.Send(new LeaveRoomCommand
        {
            RoomId = roomId,
            UserId = Context.User.GetSubjectId()
        });

        var leave = response.EnsureSuccess();

        if (leave.HasUserLeft)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync(new LeaveRoomMessage
            {
                RoomId = leave.Room.Id,
                RoomName = leave.Room.Name,
                UserId = leave.User.Id,
                UserName = leave.User.Name,
            });
        }

        return leave;
    }

    public async Task SendContentMessageAsync(string roomId, string contentType, byte[] content)
    {
        await Clients.Group(roomId).SendAsync(new ContentMessage
        {
            RoomId = roomId,
            UserId = Context.User.GetSubjectId(),
            UserName = Context.User.GetDisplayName(),
            ContentType = contentType,
            Content = content,
        });
    }

    public async Task<GetUsersLoggedInResponse> GetUsersLoggedInAsync()
    {
        var response = await _mediator.Send(new GetUsersLoggedInQuery());

        return response.EnsureSuccess();
    }

    public async Task<GetUsersLoggedInRoomResponse> GetUsersLoggedInRoomAsync(string roomId)
    {
        var response = await _mediator.Send(new GetUsersLoggedInRoomQuery { RoomId = roomId });
        return response.EnsureSuccess();
    }

    public async Task<GetRoomsWithUserResponse> GetRoomsWithUserAsync()
    {
        var response = await _mediator.Send(new GetRoomsWithUserQuery { UserId = Context.User.GetSubjectId() });
        return response.EnsureSuccess();
    }

    public async Task<GetRoomsWithoutUserResponse> GetRoomsWithoutUserAsync()
    {
        var response = await _mediator.Send(new GetRoomsWithoutUserQuery { UserId = Context.User.GetSubjectId() });
        return response.EnsureSuccess();
    }
}

file static class ClientProxyExtensions
{
    public static Task SendAsync<TMessage>(this IClientProxy clientProxy, TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        return clientProxy.SendAsync(typeof(TMessage).Name, message, cancellationToken);
    }
}
