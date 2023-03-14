namespace LetsTalk.Messaging;

public sealed class JoinRoomMessage : Message
{
    public required string RoomId { get; init; }
    public required string RoomName { get; init; }

    public required string UserId { get; init; }
    public required string UserName { get; init; }
}