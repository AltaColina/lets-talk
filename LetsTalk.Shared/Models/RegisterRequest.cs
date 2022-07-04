using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models;

public sealed class RegisterRequest : IRequest<AuthenticationResponse>
{
    [NotNull] public string? Username { get; init; }
    [NotNull] public string? Password { get; init; }
}
