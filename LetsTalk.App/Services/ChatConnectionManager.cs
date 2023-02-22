using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.App.Models;
using LetsTalk.Chats;
using System.Collections.ObjectModel;

namespace LetsTalk.App.Services;

public sealed class ChatConnectionManager
{
    private readonly IMessenger _messenger;

	public ObservableCollection<ChatConnection> Connections { get; private set; } = new();

    public ChatConnectionManager(IMessenger messenger)
    {
        _messenger = messenger;
    }

	public void Add(ChatDto chat) => Connections.Add(new(_messenger, chat));

    public void Remove(ChatDto chat) => Connections.Remove(new(_messenger, chat));

    public void Reset(IEnumerable<ChatDto> chats) => Connections = new(chats.Select(chat => new ChatConnection(_messenger, chat)));
}
