using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Dtos.Users;

public sealed class DeleteUserRequest : IRequest
{
    [NotNull] public string? Id { get; init; }
}
