using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public class User
{
    [NotNull] public string? Id { get; init; }
    [NotNull] public string? Secret { get; init; }
    public HashSet<string> Roles { get; init; } = new();
    public DateTimeOffset CreationTime { get; init; }
    public DateTimeOffset LastLoginTime { get; set; }
    public List<Token> RefreshTokens { get; init; } = new();
}
