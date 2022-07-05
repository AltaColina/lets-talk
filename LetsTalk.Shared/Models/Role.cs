using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class Role
{
    [NotNull] public string? Id { get; init; }
    public List<string> Permissions { get; init; } = new();
}
