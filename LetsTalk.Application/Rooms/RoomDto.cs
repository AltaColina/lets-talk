using LetsTalk.Interfaces;

namespace LetsTalk.Rooms;

public sealed class RoomDto : IMapFrom<Room>
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? ImageUrl { get; init; }
}