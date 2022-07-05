using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class RolePostRequest : IRequest
{
    [NotNull] public Role? Role { get; init; }
}