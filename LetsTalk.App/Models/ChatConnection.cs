using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.Chats;
using LetsTalk.Messaging;
using System.Collections.ObjectModel;

namespace LetsTalk.App.Models;
public sealed partial class ChatConnection : ObservableObject, IRecipient<ContentMessage>
{
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private int _newMessageCount;

    [ObservableProperty]
    private bool _isChatVisible;

    public ChatDto Chat { get; }

    public ObservableCollection<ContentMessage> Messages { get; } = new();

    public ChatConnection(IMessenger messenger, ChatDto chat)
    {
        _messenger = messenger;
        Chat = chat;
        _messenger.Register(this, chat.Id);
    }

    partial void OnIsChatVisibleChanged(bool value)
    {
        if (value)
            NewMessageCount = 0;
    }

    void IRecipient<ContentMessage>.Receive(ContentMessage message)
    {
        Messages.Add(message);
        if (!IsChatVisible)
            NewMessageCount++;
    }
}
