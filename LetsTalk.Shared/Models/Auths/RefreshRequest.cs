using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Auths;

public sealed class RefreshRequest : IRequest<AuthenticationResponse>
{
    [NotNull] public string? Username { get; init; }
    [NotNull] public string? RefreshToken { get; init; }
}
