using Ardalis.Specification;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LetsTalk.Models;

public class User : IEntity<string>
{
    [NotNull] public string? Id { get; set; }
    [NotNull, JsonIgnore] public string? Secret { get; set; }
    public HashSet<string> Roles { get; set; } = new();
    public HashSet<string> Chats { get; set; } = new();
    public DateTimeOffset CreationTime { get; set; }
    public DateTimeOffset LastLoginTime { get; set; }
    [JsonIgnore] public List<Token> RefreshTokens { get; set; } = new();
}
