using LetsTalk.Rooms;
using LetsTalk.Users;

namespace LetsTalk.Messaging;

public sealed class JoinRoomMessage : Message<UserDto>
{
    public RoomDto Room { get; init; } = null!;
}