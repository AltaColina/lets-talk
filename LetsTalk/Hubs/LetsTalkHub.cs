using LetsTalk.Commands.Hubs;
using LetsTalk.Models;
using LetsTalk.Queries.Chats;
using LetsTalk.Queries.Hubs;
using LetsTalk.Queries.Users;
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
        var user = await _mediator.Send(new ConnectRequest
        {
            ConnectionId = Context.ConnectionId,
            UserId = Context.User?.Identity?.Name!
        });

        await Task.WhenAll(user.Chats.Select(chatId => Groups.AddToGroupAsync(Context.ConnectionId, chatId)));
        await Clients.Others.SendAsync(new ConnectMessage { Content = user });
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await _mediator.Send(new DisconnectRequest
        {
            ConnectionId = Context.ConnectionId,
            UserId = Context.User?.Identity?.Name!
        });
        await Task.WhenAll(user.Chats.Select(chatId => Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId)));
        await Clients.Others.SendAsync(new DisconnectMessage { Content = user });
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChatAsync(string chatId)
    {
        var response = await _mediator.Send(new JoinChatRequest
        {
            ChatId = chatId,
            UserId = Context.User?.Identity?.Name!
        });

        if (response.HasUserJoined)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            await Clients.Group(response.Chat.Id).SendAsync(new JoinChatMessage
            {
                Chat = response.Chat,
                Content = response.User
            });
        }
    }

    public async Task LeaveChatAsync(string chatId)
    {
        var response = await _mediator.Send(new LeaveChatRequest
        {
            ChatId = chatId,
            UserId = Context.User?.Identity?.Name!
        });

        if (response.HasUserLeft)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
            await Clients.Group(chatId).SendAsync(new LeaveChatMessage
            {
                Chat = response.Chat,
                Content = response.User
            });
        }
    }

    public async Task SendChatMessageAsync(string chatId, string content)
    {
        var user = await _mediator.Send(new GetUserByIdCachedRequest { UserId = Context.User?.Identity?.Name! });
        await Clients.Group(chatId).SendAsync(new TextMessage
        {
            Sender = user,
            ChatId = chatId,
            Content = content,
        });
    }

    public async Task<GetLoggedUsersResponse> GetLoggedUsersAsync()
    {
        var response = await _mediator.Send(new GetLoggedUsersRequest());
        return response;
    }

    public async Task<GetLoggedChatUsersResponse> GetLoggedChatUsersAsync(string chatId)
    {
        var response = await _mediator.Send(new GetLoggedChatUsersRequest { ChatId = chatId });
        return response;
    }

    public async Task<GetUserChatsResponse> GetUserChatsAsync()
    {
        var response = await _mediator.Send(new GetUserChatsRequest { UserId = Context.User?.Identity?.Name! });
        return response;
    }

    public async Task<GetUserAvailableChatsResponse> GetUserAvailableChatsAsync()
    {
        var response = await _mediator.Send(new GetUserAvailableChatsRequest { UserId = Context.User?.Identity?.Name! });
        return response;
    }
}

internal static class ClientProxyExtensions
{
    public static Task SendAsync<TMessage>(this IClientProxy clientProxy, TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        return clientProxy.SendCoreAsync(typeof(TMessage).Name, new[] { message }, cancellationToken);
    }
}
