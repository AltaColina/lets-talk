using Ardalis.Specification;

namespace LetsTalk.Models;

public sealed class Token : IEntity<string>
{
    public string Id { get; set; } = null!;
    public DateTimeOffset ExpiresIn { get; set; }
}