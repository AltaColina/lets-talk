namespace LetsTalk.Rooms;

public sealed class Room : Entity
{
    public HashSet<string> Users { get; init; } = new();
}
