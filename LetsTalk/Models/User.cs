using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public class User
{
    [NotNull] public string? Id { get; init; }
    [NotNull] public string? Secret { get; init; }
    public List<Role> Roles { get; init; } = new();
    public DateTime CreationTime { get; init; }
    public DateTime LastLoginTime { get; set; }
    public List<Token> RefreshTokens { get; init; } = new();
}
