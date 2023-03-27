using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.Messaging;
using System.Net.Mime;
using System.Text;

namespace LetsTalk;

internal sealed class MessageRecipient :
    IRecipient<ConnectMessage>,
    IRecipient<DisconnectMessage>,
    IRecipient<JoinRoomMessage>,
    IRecipient<LeaveRoomMessage>,
    IRecipient<ContentMessage>
{
    private readonly IMessenger _messenger;

    public event EventHandler<string>? MessageReceived;

    public MessageRecipient(IMessenger messenger)
    {
        _messenger = messenger;
        _messenger.Register<ConnectMessage>(this);
        _messenger.Register<DisconnectMessage>(this);
        _messenger.Register<JoinRoomMessage>(this);
        _messenger.Register<LeaveRoomMessage>(this);
    }

    private void NotifyMessage(string message) => MessageReceived?.Invoke(this, message);

    public void AddMessageListener(string roomId) => _messenger.Register<ContentMessage, string>(this, roomId);

    public void RemoveMessageListener(string roomId) => _messenger.Unregister<ContentMessage, string>(this, roomId);

    public void RemoveEventHandlers() => MessageReceived = null;

    void IRecipient<ConnectMessage>.Receive(ConnectMessage message) => NotifyMessage($"User '{message.UserName}' has connected to the server.");
    void IRecipient<DisconnectMessage>.Receive(DisconnectMessage message) => NotifyMessage($"User '{message.UserName}' has disconnected from the server.");
    void IRecipient<JoinRoomMessage>.Receive(JoinRoomMessage message) => NotifyMessage($"User '{message.UserName}' has joined channel '{message.RoomId}'.");
    void IRecipient<LeaveRoomMessage>.Receive(LeaveRoomMessage message) => NotifyMessage($"User '{message.UserName}' has left channel '{message.RoomId}'.");
    void IRecipient<ContentMessage>.Receive(ContentMessage message)
    {
        switch (message.ContentType)
        {
            case MediaTypeNames.Text.Plain:
                NotifyMessage($"{message.UserName}: {Encoding.UTF8.GetString(message.Content)}");
                return;

            default:
                NotifyMessage($"{message.UserName}: {message.ContentType} ({message.Content.Length} bytes)");
                return;
        }
    }
}
