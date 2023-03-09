using LetsTalk.Interfaces;

namespace LetsTalk.Rooms;

public sealed class RoomDto : IMapFrom<Room>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
}