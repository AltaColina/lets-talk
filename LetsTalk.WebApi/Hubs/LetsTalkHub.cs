using LetsTalk.Hubs.Commands;
using LetsTalk.Hubs.Queries;
using LetsTalk.Messaging;
using LetsTalk.Users.Queries;
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
        var user = await _mediator.Send(new ConnectCommand
        {
            ConnectionId = Context.ConnectionId,
            UserId = Context.User!.ReadUserIdClaim()
        });

        await Task.WhenAll(user.Rooms.Select(roomId => Groups.AddToGroupAsync(Context.ConnectionId, roomId)));
        await Clients.Others.SendAsync(new ConnectMessage
        {
            UserId = user.Id,
            UserName = user.Name,
            UserImageUrl = user.ImageUrl,
        });
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await _mediator.Send(new DisconnectCommand
        {
            ConnectionId = Context.ConnectionId,
            UserId = Context.User!.ReadUserIdClaim()
        });
        await Task.WhenAll(user.Rooms.Select(roomId => Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId)));
        await Clients.Others.SendAsync(new DisconnectMessage
        {
            UserId = user.Id,
            UserName = user.Name,
            UserImageUrl = user.ImageUrl
        });
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<JoinRoomResponse> JoinRoomAsync(string roomId)
    {
        var response = await _mediator.Send(new JoinRoomCommand
        {
            RoomId = roomId,
            UserId = Context.User!.ReadUserIdClaim()
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
            UserId = Context.User!.ReadUserIdClaim()
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
        var user = Context.User!.ReadUserClaims();
        await Clients.Group(roomId).SendAsync(new ContentMessage
        {
            RoomId = roomId,
            UserId = user.UserId,
            UserName = user.UserName,
            ContentType = contentType,
            Content = content,
        });
    }

    public async Task<GetLoggedUsersResponse> GetLoggedUsersAsync()
    {
        var response = await _mediator.Send(new GetLoggedUsersQuery());
        return response;
    }

    public async Task<GetLoggedRoomUsersResponse> GetLoggedRoomUsersAsync(string roomId)
    {
        var response = await _mediator.Send(new GetLoggedRoomUsersQuery { RoomId = roomId });
        return response;
    }

    public async Task<GetUserRoomsResponse> GetUserRoomsAsync()
    {
        var response = await _mediator.Send(new GetUserRoomsQuery { UserId = Context.User?.Identity?.Name! });
        return response;
    }

    public async Task<GetUserAvailableRoomsResponse> GetUserAvailableRoomsAsync()
    {
        var response = await _mediator.Send(new GetUserAvailableRoomsQuery { UserId = Context.User?.Identity?.Name! });
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
