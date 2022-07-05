namespace LetsTalk.Models;

public sealed class RoleUserGetResponse
{
    public List<User> Users { get; init; } = new();
}
