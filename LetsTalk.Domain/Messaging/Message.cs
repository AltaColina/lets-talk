namespace LetsTalk.Messaging;

public abstract class Message
{
    public string Id { get; init; }

    public MessageType Type { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    protected Message()
    {
        Id = Guid.NewGuid().ToString();
        Type = GetType().Name switch
        {
            nameof(ConnectMessage) => MessageType.Connect,
            nameof(DisconnectMessage) => MessageType.Disconnect,
            nameof(JoinRoomMessage) => MessageType.JoinRoom,
            nameof(LeaveRoomMessage) => MessageType.LeaveRoom,
            nameof(ContentMessage) => MessageType.Content,
            _ => throw new InvalidOperationException($"Type {GetType().Name} has not enumeration value in {nameof(MessageType)}"),
        };
        Timestamp = DateTimeOffset.UtcNow;
    }
}