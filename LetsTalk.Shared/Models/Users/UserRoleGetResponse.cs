namespace LetsTalk.Models.Users;

public sealed class UserRoleGetResponse
{
    public List<Role> Roles { get; init; } = new();
}