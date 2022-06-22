using LiteDB;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace LetsTalk.Models;

public class User
{
    public string Id { get => Username; }
    [NotNull] public string? Username { get; init; }
    [NotNull] public string? Password { get; init; }
    public DateTime CreationTime { get; init; }
    public DateTime LastLoginTime { get; set; }
}
