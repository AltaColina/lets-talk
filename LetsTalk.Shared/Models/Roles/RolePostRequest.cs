using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Roles;

public sealed class RolePostRequest : IRequest
{
    [NotNull] public Role? Role { get; init; }
}