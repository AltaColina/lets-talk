using Ardalis.Specification;

namespace LetsTalk.Rooms;

public sealed class Room : IEntity<string>
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public HashSet<string> Users { get; init; } = new();
}
