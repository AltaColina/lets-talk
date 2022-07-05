using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class RoleDeleteRequest : IRequest
{
    [NotNull] public string? RoleId { get; init; }
}