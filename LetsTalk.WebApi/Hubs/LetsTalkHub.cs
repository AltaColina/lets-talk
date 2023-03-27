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

        await Task.WhenAll(response.Rooms.Select(roomId => Groups.AddToGroupAsync(Context.ConnectionId, roomId)));
        await Clients.Others.SendAsync(new ConnectMessage
        {
            UserId = response.User.Id,
            UserName = response.User.Name,
            UserImage = response.User.Image,
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

        await Task.WhenAll(response.Rooms.Select(roomId => Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId)));
        await Clients.Others.SendAsync(new DisconnectMessage
        {
            UserId = response.User.Id,
            UserName = response.User.Name,
            UserImage = response.User.Image
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

        if (response.HasUserJoined)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(response.Room.Id).SendAsync(new JoinRoomMessage
            {
                RoomId = response.Room.Id,
                RoomName = response.Room.Name,
                UserId = response.User.Id,
                UserName = response.User.Name,
            });
        }

        return response;
    }

    public async Task<LeaveRoomResponse> LeaveRoomAsync(string roomId)
    {
        var response = await _mediator.Send(new LeaveRoomCommand
        {
            RoomId = roomId,
            UserId = Context.User.GetSubjectId()
        });

        if (response.HasUserLeft)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync(new LeaveRoomMessage
            {
                RoomId = response.Room.Id,
                RoomName = response.Room.Name,
                UserId = response.User.Id,
                UserName = response.User.Name,
            });
        }

        return response;
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
        return response;
    }

    public async Task<GetUsersLoggedInRoomResponse> GetUsersLoggedInRoomAsync(string roomId)
    {
        var response = await _mediator.Send(new GetUsersLoggedInRoomQuery { RoomId = roomId });
        return response;
    }

    public async Task<GetRoomsWithUserResponse> GetRoomsWithUserAsync()
    {
        var response = await _mediator.Send(new GetRoomsWithUserQuery { UserId = Context.User.GetSubjectId() });
        return response;
    }

    public async Task<GetRoomsWithoutUserResponse> GetRoomsWithoutUserAsync()
    {
        var response = await _mediator.Send(new GetRoomsWithoutUserQuery { UserId = Context.User.GetSubjectId() });
        return response;
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
