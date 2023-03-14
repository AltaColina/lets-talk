namespace LetsTalk.Rooms;

public sealed class Room : Entity
{
    public string? ImageUrl { get; set; }

    public HashSet<string> Users { get; init; } = new();
}
