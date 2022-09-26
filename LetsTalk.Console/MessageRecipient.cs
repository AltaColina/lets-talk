using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.Messaging;
using LetsTalk.Models;

namespace LetsTalk.Console;
internal sealed class MessageRecipient :
    IRecipient<ConnectMessage>,
    IRecipient<DisconnectMessage>,
    IRecipient<JoinChatMessage>,
    IRecipient<LeaveChatMessage>,
    IRecipient<TextMessage>
{
    private readonly IMessenger _messenger;

    public event EventHandler<string>? MessageReceived;

    public MessageRecipient(IMessenger messenger)
    {
        _messenger = messenger;
        _messenger.Register<ConnectMessage>(this);
        _messenger.Register<DisconnectMessage>(this);
        _messenger.Register<JoinChatMessage>(this);
        _messenger.Register<LeaveChatMessage>(this);
    }

    private void NotifyMessage(string message) => MessageReceived?.Invoke(this, message);

    public void ListenToChat(string chatId) => _messenger.Register<TextMessage, string>(this, chatId);

    void IRecipient<ConnectMessage>.Receive(ConnectMessage message) => NotifyMessage($"User '{message.Content.Id}' has connected to the server.");
    void IRecipient<DisconnectMessage>.Receive(DisconnectMessage message) => NotifyMessage($"User '{message.Content.Id}' has disconnected from the server.");
    void IRecipient<JoinChatMessage>.Receive(JoinChatMessage message) => NotifyMessage($"User '{message.Content.Id}' has joined channel '{message.Chat.Name}'.");
    void IRecipient<LeaveChatMessage>.Receive(LeaveChatMessage message) => NotifyMessage($"User '{message.Content.Id}' has left channel '{message.Chat.Name}'.");
    void IRecipient<TextMessage>.Receive(TextMessage message) => NotifyMessage($"{message.Sender.Name}: {message.Content}");
}
