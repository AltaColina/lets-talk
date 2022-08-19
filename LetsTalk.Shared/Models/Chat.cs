using Ardalis.Specification;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class Chat : IEntity<string>
{
    [NotNull] public string? Id { get; set; }
}
