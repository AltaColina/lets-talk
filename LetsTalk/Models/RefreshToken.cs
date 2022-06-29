using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class RefreshToken
{
    [NotNull] public string? Id { get; init; }
    public DateTime ExpiresIn { get; init; }
}
