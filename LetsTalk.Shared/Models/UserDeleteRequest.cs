using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class UserDeleteRequest : IRequest
{
    [NotNull] public string? UserId { get; init; }
}
