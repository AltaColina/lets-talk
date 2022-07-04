namespace LetsTalk.Models;

public sealed class RoleGetResponse
{
    public List<Role> Roles { get; init; } = new();
}