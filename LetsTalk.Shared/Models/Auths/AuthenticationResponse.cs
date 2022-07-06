using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Models.Auths;

public sealed class AuthenticationResponse
{
    [NotNull] public Token? AccessToken { get; init; }
    [NotNull] public Token? RefreshToken { get; init; }
    [NotNull] public Person? Person { get; init; }
}