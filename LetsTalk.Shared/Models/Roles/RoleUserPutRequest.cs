using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Roles;

public sealed class RoleUserPutRequest : IRequest
{
    [NotNull] public string? RoleId { get; init; }

    [NotNull] public string? UserId { get; init; }
}
