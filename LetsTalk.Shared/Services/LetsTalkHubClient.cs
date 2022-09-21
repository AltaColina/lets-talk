using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace LetsTalk.App.Services;

internal sealed class LetsTalkHubClient : ILetsTalkHubClient
{
    private readonly ConcurrentDictionary<string, ObservableCollection<ChatMessage>> _messages = new();
    private readonly IMessenger _messenger;
    private readonly IConfiguration _configuration;
    private HubConnection? _connection;

    public bool IsConnected { get; private set; }

    public LetsTalkHubClient(IMessenger messenger, IConfiguration configuration)
    {
        _messenger = messenger;
        _configuration = configuration;
    }

    public async Task ConnectAsync(Func<Task<string?>> provideToken)
    {
        if (_connection is not null)
            await _connection.DisposeAsync();

        var address = _configuration["LetsTalkHubAddress"];

        _connection = new HubConnectionBuilder()
            .WithUrl(address, opts => opts.AccessTokenProvider = provideToken)
            .Build();
        _connection.On<ChatMessage>(Handle);
        await _connection.StartAsync();
        IsConnected = true;
    }

    public async Task DisconnectAsync()
    {
        if (_connection is not null)
            await _connection.StopAsync();
        IsConnected = false;
    }

    private void Handle(ChatMessage message)
    {
        _messenger.Send(message);
        GetChatMessages(message.ChatId).Add(message);
    }

    public ObservableCollection<ChatMessage> GetChatMessages(string chatId) => _messages.GetOrAdd(chatId, _ => new ObservableCollection<ChatMessage>());

    public async Task JoinChatAsync(string chatId) => await _connection!.InvokeAsync(nameof(JoinChatAsync), chatId);
    public async Task LeaveChatAsync(string chatId) => await _connection!.InvokeAsync(nameof(LeaveChatAsync), chatId);

    public async Task SendChatMessageAsync(string chatId, string message) => await _connection!.InvokeAsync(nameof(SendChatMessageAsync), chatId, message);
}
internal static partial class HubConnectionExtensions
{
    public static IDisposable On<TMessage>(this HubConnection hubConnection, Action<TMessage> handler)
    {
        return hubConnection.On(typeof(TMessage).Name, handler);
    }
}
