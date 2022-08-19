using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class Person
{
    [NotNull] public string? Username { get; set; }
}