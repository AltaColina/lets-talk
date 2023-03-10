using LetsTalk.Users;

namespace LetsTalk.Security;

public sealed class Authentication
{
    public required UserDto User { get; init; }

    public required string AccessToken { get; init; }

    public required DateTimeOffset AccessTokenExpiresIn { get; init; }

    public required string RefreshToken { get; init; }

    public required DateTimeOffset RefreshTokenExpiresIn { get; init; }

    public required HashSet<string> Permissions { get; init; }
}