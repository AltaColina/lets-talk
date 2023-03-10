using LetsTalk.Users;

namespace LetsTalk.Messaging;

public sealed class JoinRoomMessage : Message
{
    public required string RoomId { get; init; }
    public required UserTag User { get; init; }
}