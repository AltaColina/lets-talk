namespace LetsTalk.Models.Roles;

public sealed class RoleGetResponse
{
    public List<Role> Roles { get; init; } = new();
}