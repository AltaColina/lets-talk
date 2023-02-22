using Ardalis.Specification;

namespace LetsTalk.Roles;

public sealed class Role : IEntity<string>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public HashSet<string> Permissions { get; set; } = new();
}
