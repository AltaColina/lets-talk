using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Users;

public sealed class UserDeleteRequest : IRequest
{
    [NotNull] public string? UserId { get; init; }
}
