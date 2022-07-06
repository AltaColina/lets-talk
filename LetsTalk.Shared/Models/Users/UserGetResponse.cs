namespace LetsTalk.Models.Users;

public sealed class UserGetResponse
{
    public List<User> Users { get; init; } = new();
}