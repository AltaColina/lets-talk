using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class RolePutRequest : IRequest
{
    [NotNull] public string? UserId { get; set; }
    public List<Role> Roles { get; init; } = new();
}