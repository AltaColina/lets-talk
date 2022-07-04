namespace LetsTalk.Models;

public sealed class UserGetResponse
{
    public List<User> Users { get; init; } = new();
}