using LetsTalk.Commands.Hubs;
using LetsTalk.Dtos;
using LetsTalk.Models;
using LetsTalk.Queries.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.Hubs;

[Authorize]
public sealed class LetsTalkHub : Hub
{
    private const string AdminId = "admin";
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
            await Clients.Group(response.Chat.Id).SendAsync(new ChatMessage
            {
                ChatId = response.Chat.Id,
                UserId = AdminId,
                Content = $"{Context.User!.Identity!.Name} has joined chat {response.Chat.Id}"
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
            await Clients.Group(chatId).SendAsync(new ChatMessage
            {
                ChatId = response.Chat.Id,
                UserId = AdminId,
                Content = $"{Context.User!.Identity!.Name} has left chat {response.Chat.Id}"
            });
        }
    }

    public async Task SendChatMessageAsync(string chatId, string content)
    {
        await Clients.Group(chatId).SendAsync(new ChatMessage
        {
            ChatId = chatId,
            UserId = Context.User!.Identity!.Name!,
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
}

internal static class ClientProxyExtensions
{
    public static Task SendAsync<TMessage>(this IClientProxy clientProxy, TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        return clientProxy.SendCoreAsync(typeof(TMessage).Name, new[] { message }, cancellationToken);
    }
}
