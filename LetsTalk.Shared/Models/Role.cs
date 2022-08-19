using Ardalis.Specification;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class Role : IEntity<string>
{
    [NotNull] public string? Id { get; set; }
    public List<string> Permissions { get; set; } = new();
}
