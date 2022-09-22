using Ardalis.Specification;

namespace LetsTalk.Models;

public sealed class Chat : IEntity<string>
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public HashSet<string> Users { get; init; } = new();
}
