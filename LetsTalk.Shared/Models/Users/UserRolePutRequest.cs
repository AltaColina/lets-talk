using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Users;

public sealed class UserRolePutRequest : IRequest
{
    [NotNull] public string? UserId { get; init; }
    public List<string> Roles { get; init; } = new();
}