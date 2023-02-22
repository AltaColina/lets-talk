using Ardalis.Specification;
using LetsTalk.Security;

namespace LetsTalk.Users;

public sealed class User : IEntity<string>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Secret { get; set; } = null!;
    public HashSet<string> Roles { get; init; } = new();
    public HashSet<string> Chats { get; init; } = new();
    public DateTimeOffset CreationTime { get; init; } = DateTimeOffset.Now;
    public DateTimeOffset LastLoginTime { get; set; }
    public List<Token> RefreshTokens { get; set; } = new();
}
