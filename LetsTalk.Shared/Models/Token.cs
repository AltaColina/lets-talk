using Ardalis.Specification;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class Token : IEntity<string>
{
    [NotNull] public string? Id { get; set; }
    public DateTimeOffset ExpiresIn { get; set; }
}