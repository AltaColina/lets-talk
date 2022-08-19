using LetsTalk.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace LetsTalk.App.Services;

public interface ILetsTalkHubClient
{
    bool IsConnected { get; }

    Task ConnectAsync(string uri, Func<Task<string?>> provideToken);

    ObservableCollection<Message> GetChatMessages(string chatId);

    Task JoinChatAsync(string chatId);
    Task LeaveChatAsync(string chatId);

    Task SendMessageAsync(string chatId, string message);
}

public class LetsTalkHubClient : ILetsTalkHubClient
{
    private readonly ConcurrentDictionary<string, ObservableCollection<Message>> _messages = new();
    private HubConnection? _connection;

    public bool IsConnected { get; private set; }


    public async Task ConnectAsync(string uri, Func<Task<string?>> provideToken)
    {
        if (_connection is not null)
            await _connection.DisposeAsync();

        _connection = new HubConnectionBuilder()
            .WithUrl(uri, opts => opts.AccessTokenProvider = provideToken)
            .Build();
        _connection.On<Message>(Methods.UserMessage, msg => GetChatMessages(msg.ChatId).Add(msg));
        await _connection.StartAsync();
    }

    public ObservableCollection<Message> GetChatMessages(string chatId) => _messages.GetOrAdd(chatId, _ => new ObservableCollection<Message>());

    public async Task JoinChatAsync(string chatId) => await _connection!.InvokeAsync(Methods.Join, chatId);
    public async Task LeaveChatAsync(string chatId) => await _connection!.InvokeAsync(Methods.Leave, chatId);

    public async Task SendMessageAsync(string chatId, string message) => await _connection!.InvokeAsync(Methods.UserMessage, chatId, message);
}
