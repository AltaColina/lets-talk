using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LetsTalk.Models;

public class User
{
    [NotNull] public string? Id { get; init; }
    [NotNull, JsonIgnore] public string? Secret { get; init; }
    public HashSet<string> Roles { get; init; } = new();
    public HashSet<string> Chats { get; init; } = new();
    public DateTimeOffset CreationTime { get; init; }
    public DateTimeOffset LastLoginTime { get; set; }
    [JsonIgnore] public List<Token> RefreshTokens { get; init; } = new();
}
