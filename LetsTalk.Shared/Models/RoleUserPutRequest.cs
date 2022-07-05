using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class RoleUserPutRequest : IRequest
{
    [NotNull] public string? RoleId { get; init; }

    [NotNull] public User? User { get; init; }
}
