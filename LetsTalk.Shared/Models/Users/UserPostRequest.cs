using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Users;

public sealed class UserPostRequest : IRequest
{
    [NotNull] public User? User { get; init; }
}
