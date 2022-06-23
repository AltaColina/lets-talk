using LiteDB;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace LetsTalk.Models;

public class User
{
    [NotNull] public string? Id { get; init; }
    [NotNull] public string? Secret { get; init; }
    public bool IsAdministrator { get; set; }
    public DateTime CreationTime { get; init; }
    public DateTime LastLoginTime { get; set; }
    public List<RefreshToken> RefreshTokens { get; init; } = new();
}
