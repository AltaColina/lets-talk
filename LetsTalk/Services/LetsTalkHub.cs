using LetsTalk.Exceptions;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.Services;

[Authorize()]
public sealed class LetsTalkHub : Hub
{
    private readonly IChatRepository _chatRepository;

    public LetsTalkHub(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Others.SendAsync(Methods.ServerMessage, new Message
        {
            Username = "Server",
            Content = $"{Context.User!.Identity!.Name} has connected."
        });
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.SendAsync(Methods.ServerMessage, new Message
        {
            Username = "Server",
            Content = $"{Context.User!.Identity!.Name} has disconnected. {(exception is not null ? exception.Message : String.Empty)}"
        });
        await base.OnDisconnectedAsync(exception);
    }

    public async Task Join(string chatId)
    {
        var chat = await _chatRepository.GetAsync(chatId);
        if (chat is null)
            throw new NotFoundException($"Chat {chatId} does not exist");
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        await Clients.Group(chat.Id).SendAsync(Methods.ServerMessage, new Message
        {
            Username = "Server",
            Content = $"{Context.User!.Identity!.Name} has joined chat {chat.Id}"
        });
    }

    public async Task Leave(string chatId)
    {
        var chat = await _chatRepository.GetAsync(chatId);
        if (chat is null)
            throw new NotFoundException($"Chat {chatId} does not exist");
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        await Clients.Group(chatId).SendAsync(Methods.ServerMessage, new Message
        {
            Username = "Server",
            Content = $"{Context.User!.Identity!.Name} has left chat {chat.Id}"
        });
    }

    public async Task UserMessage(string chatId, string content)
    {
        await Clients.Group(chatId).SendAsync(Methods.UserMessage, new Message
        {
            Username = Context.User!.Identity!.Name,
            Content = content,
        });
    }
}
