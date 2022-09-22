using Ardalis.Specification;

namespace LetsTalk.Models;

public sealed class User : IEntity<string>
{
    public string Id { get; set; } = null!;
    public string Secret { get; set; } = null!;
    public HashSet<string> Roles { get; init; } = new();
    public HashSet<string> Chats { get; init; } = new();
    public DateTimeOffset CreationTime { get; set; }
    public DateTimeOffset LastLoginTime { get; set; }
    public List<Token> RefreshTokens { get; set; } = new();
}
