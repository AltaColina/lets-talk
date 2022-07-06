namespace LetsTalk.Models.Roles;

public sealed class RoleUserGetResponse
{
    public List<User> Users { get; init; } = new();
}
