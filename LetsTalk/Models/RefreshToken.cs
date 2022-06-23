namespace LetsTalk.Models;

public sealed class RefreshToken
{
    public string Id { get; init; }
    public DateTime ExpiresIn { get; init; }
}
