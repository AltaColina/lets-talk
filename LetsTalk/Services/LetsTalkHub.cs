using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.Services;

[Authorize]
public sealed class LetsTalkHub : Hub
{
    private const string AdminId = "admin";
    private readonly IHubConnectionMapper _connectionMapper;
    private readonly IRepository<Chat> _chatRepository;

    public LetsTalkHub(IHubConnectionMapper connectionMapper, IRepository<Chat> chatRepository)
    {
        _connectionMapper = connectionMapper;
        _chatRepository = chatRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        var userId = Context.User!.Identity!.Name!;
        _connectionMapper.AddMapping(connectionId, userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        _connectionMapper.RemoveMapping(connectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChatAsync(string chatId)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat is null)
            throw new NotFoundException($"Chat {chatId} does not exist");
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        await Clients.Group(chat.Id).SendAsync(new ChatMessage
        {
            ChatId = chat.Id,
            UserId = AdminId,
            Content = $"{Context.User!.Identity!.Name} has joined chat {chat.Id}"
        });
    }

    public async Task LeaveChatAsync(string chatId)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat is null)
            throw new NotFoundException($"Chat {chatId} does not exist");
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        await Clients.Group(chatId).SendAsync(new ChatMessage
        {
            ChatId = chat.Id,
            UserId = AdminId,
            Content = $"{Context.User!.Identity!.Name} has left chat {chat.Id}"
        });
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
}

internal static class ClientProxyExtensions
{
    public static Task SendAsync<TMessage>(this IClientProxy clientProxy, TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        return clientProxy.SendCoreAsync(typeof(TMessage).Name, new[] { message }, cancellationToken);
    }
}
