using LetsTalk.Rooms;
using LetsTalk.Users;

namespace LetsTalk.Messaging;

public sealed class LeaveRoomMessage : Message<UserDto>
{
    public RoomDto Room { get; init; } = null!;
}
