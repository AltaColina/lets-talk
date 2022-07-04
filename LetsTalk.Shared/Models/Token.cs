using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class Token
{
    [NotNull] public string? Id { get; init; }
    public DateTimeOffset ExpiresIn { get; init; }
}