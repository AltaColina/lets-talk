namespace LetsTalk.Roles;

public sealed class Role : Entity
{
    public HashSet<string> Permissions { get; set; } = new();
}
