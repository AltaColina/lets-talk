namespace LetsTalk.Users;

public sealed class User : Entity
{
    [SensitiveData]
    public required string Secret { get; set; }
    public string? ImageUrl { get; set; }
    public DateTimeOffset LastLoginTime { get; set; }
    public HashSet<string> Roles { get; init; } = new();
    public HashSet<string> Rooms { get; init; } = new();
    public HashSet<string> RefreshTokens { get; set; } = new();
}
