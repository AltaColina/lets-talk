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
            UserId = Context.User?.Identity?.Name!
        });

        await Task.WhenAll(user.Chats.Select(chatId => Groups.AddToGroupAsync(Context.ConnectionId, chatId)));
        await Clients.Others.SendAsync(new ConnectMessage { Content = user });
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await _mediator.Send(new DisconnectCommand
        {
            ConnectionId = Context.ConnectionId,
            UserId = Context.User?.Identity?.Name!
        });
        await Task.WhenAll(user.Chats.Select(chatId => Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId)));
        await Clients.Others.SendAsync(new DisconnectMessage { Content = user });
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<JoinChatResponse> JoinChatAsync(string chatId)
    {
        var response = await _mediator.Send(new JoinChatCommand
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

        return response;
    }

    public async Task<LeaveChatResponse> LeaveChatAsync(string chatId)
    {
        var response = await _mediator.Send(new LeaveChatCommand
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

        return response;
    }

    public async Task SendChatMessageAsync(string chatId, string contentType, byte[] content)
    {
        var user = await _mediator.Send(new GetUserByIdCachedQuery { UserId = Context.User?.Identity?.Name! });
        await Clients.Group(chatId).SendAsync(new ContentMessage
        {
            Sender = user,
            ChatId = chatId,
            ContentType = contentType,
            Content = content,
        });
    }

    public async Task<GetLoggedUsersResponse> GetLoggedUsersAsync()
    {
        var response = await _mediator.Send(new GetLoggedUsersQuery());
        return response;
    }

    public async Task<GetLoggedChatUsersResponse> GetLoggedChatUsersAsync(string chatId)
    {
        var response = await _mediator.Send(new GetLoggedChatUsersQuery { ChatId = chatId });
        return response;
    }

    public async Task<GetUserChatsResponse> GetUserChatsAsync()
    {
        var response = await _mediator.Send(new GetUserChatsQuery { UserId = Context.User?.Identity?.Name! });
        return response;
    }

    public async Task<GetUserAvailableChatsResponse> GetUserAvailableChatsAsync()
    {
        var response = await _mediator.Send(new GetUserAvailableChatsQuery { UserId = Context.User?.Identity?.Name! });
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
