using Ardalis.Specification;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class Chat : IEntity<string>
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;
}
